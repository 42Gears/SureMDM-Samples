using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Uninstall_app
{
    class Uninstall_app
    {
        public static string baseurl = "https://suremdm.42gears.com/api";
        private static string Username = "Username";
        private static string Password = "Password";
        private static string ApiKey = "Your ApiKey";
        static void Main(string[] args)
        {

            // Retrieve Device ID
            string DeviceID = GetDeviceID("Device_Name");
            if (DeviceID != null)
            {
                // Uninstall app
                string status = UninstallApplication(DeviceID, "Application_Name");
                Console.WriteLine(status);
            }
            else
            {
                Console.WriteLine("Device not found!");
            }
            Console.ReadKey();
        }

        private static string UninstallApplication(string deviceID, string appName)
        {
            /* Retrieve AppID (Package name) of an app
               Endpoint: /dynamicjob
                 Method: POST
                 Request Body:
                    {
                        "DeviceID": string,
                        "JobType": string,
                        "PayLoad": string
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
            // Create job specific PayLoad
            var PayLoad = new
            {
                AppIds = new string[] { GetAppID(deviceID, appName) }
            };
            // convert payload to base64 string
            var PayLoadbytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(PayLoad));
            var PayLoadBase64 = System.Convert.ToBase64String(PayLoadbytes);
            // Request payload for refreshing device
            var RequestPayLoad = new
            {
                JobType = "UNINSTALL_APPLICATION",
                DeviceID = deviceID,
                PayLoad = PayLoadBase64
            };
            // Add request body
            request.AddJsonBody(RequestPayLoad);
            // Execute request
            IRestResponse response = client.Execute(request);

            return response.Content.ToString();
        }

        private static string GetAppID(string deviceID, string appName)
        {
            /* Retrieve AppID (Package name) of an app
               Endpoint: /installedapp/android/{DeviceId}/device
                 Method: GET
                 Path parameters:
                     DeviceID: ID of the device allocated by the SureMDM.
                 Authentication:
                     Basic authentication
                 Headers:
                     ApiKey: “Your Api-Key”
              */
            string URL = baseurl + "/installedapp/android/" + deviceID + "/device";
            var client = new RestClient(URL);
            // Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            // ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            // Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.GET);
            // Execute request
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var OutPut = JsonConvert.DeserializeObject(response.Content);
                List<JObject> jsonResponse = JsonConvert.DeserializeObject<List<JObject>>(response.Content);
                var a = jsonResponse.Any();
                if ((jsonResponse != null) && (jsonResponse.Any()))
                {
                    foreach (var app in jsonResponse)
                    {
                        if (app.GetValue("Name").ToString().ToUpper() == appName.ToUpper())
                        {
                            return app.GetValue("Id").ToString();
                        }
                    }
                }
            }
            return null;
        }

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
