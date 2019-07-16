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

namespace Refresh_device_apps
{
    class Refresh_device_apps
    {
        public static string baseurl = "https://suremdm.42gears.com/api";
        private static string Username = "sa";
        private static string Password = "0000";
        private static string ApiKey = "37A007A7-5D12-4D14-974E-B211F8F378EA";
        static void Main(string[] args)
        {
            string DeviceID = GetDeviceID("Client0437");
            if (DeviceID != null)
            {
                // Refreshing device app info
                string status = RefreshDeviceApps(DeviceID);
                Console.WriteLine(status);
            }
            else
            {
                Console.WriteLine("Device not found!");
            }
            Console.ReadKey();
        }

        // Function to refresh device app info
        private static string RefreshDeviceApps(string deviceID)
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

            // Request payload for refreshing device
            var RequestPayLoad = new
            {
                JobType = "GET_DEVICE_APPS",
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
