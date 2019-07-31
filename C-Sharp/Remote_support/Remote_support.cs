using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Remote_support
{
    class Remote_support
    {
        public static string baseurl = "https://suremdm.42gears.com/api"; // BaseURL of SureMDM
        private static string Username = "Username";
        private static string Password = "Password";
        private static string ApiKey = "Your Api-Key";
        static void Main(string[] args)
        {
            // Generate remote support URL for device
            string URL = GetRemoteSupportURL("Device_name");
            Console.WriteLine(URL);
            Console.ReadKey();
        }
        private static string GetRemoteSupportURL(string deviceName)
        {
            // Retrieving ID of the device
            string DeviceID = GetDeviceID(deviceName);

            /*  Retreiving Id, name, uerID, pltFrmType, agentversion of the device
                 Endpoint: /api/device/{DeviceID}:
                 Method: GET
                 Authentication:
                     Basic authentication
                 Headers:
                     ApiKey: “Your Api-Key” 
            */
            string URL = baseurl+"/device/"+DeviceID;
            var client = new RestClient(URL);
            // Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            // ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            // Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.GET);
            // Getting response
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var OutPut = JsonConvert.DeserializeObject(response.Content);
                List<JObject> jsonResponse = JsonConvert.DeserializeObject<List<JObject>>(response.Content);
                var a = jsonResponse.Any();
                if ((jsonResponse != null) && (jsonResponse.Any()))
                {
                    foreach (var device in jsonResponse)
                    {
                        if (device.GetValue("DeviceName").ToString() == deviceName)
                        {
                            string remoteSupporturl = "https://suremdm.42gears.com" + "/RemoteSupport.aspx?" +
                                "id=" + device.GetValue("DeviceID").ToString() +
                                "&name=" + device.GetValue("DeviceName").ToString() +
                                "&userid=" + device.GetValue("UserID").ToString() +
                                "&pltFrmType=" + device.GetValue("PlatformType").ToString() +
                                "&agentversion=" + device.GetValue("AgentVersion").ToString() +
                                "&perm=126,127,128,129";

                            return remoteSupporturl;
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
