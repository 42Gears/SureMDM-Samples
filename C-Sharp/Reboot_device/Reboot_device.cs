using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Reboot_device
{
    class Reboot_device
    {
        public static string baseurl = "https://suremdm.42gears.com/api";
        private static string Username = "Username";
        private static string Password = "Password";
        private static string ApiKey = "Your ApiKey";
        static void Main(string[] args)
        {
            string DeviceID = GetDeviceID("Device_Name");
            if (DeviceID != null)
            {
                // Rebooting device app info
                string status = RebootDevice(DeviceID);
                Console.WriteLine(status);
            }
            else
            {
                Console.WriteLine("Device not found!");
            }
            Console.ReadKey();
        }

        // Function to reboot device app info
        private static string RebootDevice(string deviceID)
        {
            /* Api to apply dynamic jobs
               Endpoint: /dynamicjob
                 Method: POST
                 Request Body:
                    {
                        "JobType":string,
                        "DeviceID":string
                    }
                 Authentication:
                     Basic authentication
                 Headers:
                     ApiKey: “Your Api-Key”
             */

            string URL = baseurl + "/dynamicjob";
            var client = new RestClient(URL);
            // Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            // ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            // Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.POST);

            // Request payload for reboot device dynamic job
            var RequestPayLoad = new
            {
                JobType = "Reboot",
                DeviceID = deviceID
            };
            request.AddJsonBody(RequestPayLoad);

            // Execute request
            IRestResponse response = client.Execute(request);

            return response.Content.ToString();
        }

        // Function to retrieve ID of the device
        private static string GetDeviceID(string deviceName)
        {
            /*  Retreiving information of device
                 Endpoint: /device:
                 Method: POST
                 Request Body:
                     {  
                        "ID": string,
	                    "SearchValue":string,
	                    "Limit":integer,
	                    "SortColumn":string,
	                    "SortOrder":string,
	                    "IsSearch":boolean,
	                    "SearchColumns":string[]
                    }
                 Authentication:
                     Basic authentication
                 Headers:
                     ApiKey: “Your Api-Key” 
             */

            string URL = baseurl + "/device";
            var client = new RestClient(URL);
            //  Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            //  ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            //  Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            //  Set request method
            var request = new RestRequest(Method.POST);
            //  Request payload
            var RequestPayLoad = new
            {
                ID = "AllDevices",
                IsSearch = true,
                Limit = 10,
                SearchColumns = new string[] { "DeviceName" },
                SearchValue = deviceName,
                SortColumn = "LastTimeStamp",
                SortOrder = "asc"
            };
            request.AddJsonBody(RequestPayLoad);

            //  Execute request
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var OutPut = JsonConvert.DeserializeObject<JObject>(response.Content);
                foreach (var device in OutPut["rows"])
                {
                    if ((string)device["DeviceName"] == deviceName)
                    {
                        return device["DeviceID"].ToString();
                    }
                }
            }
            return null;
        }
    }
}
