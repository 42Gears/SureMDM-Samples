using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Reapply_pending_job
{
    class Reapply_job
    {
        public static string baseurl = "https://suremdm.42gears.com/api";
        private static string Username = "sa";
        private static string Password = "0000";
        private static string ApiKey = "37A007A7-5D12-4D14-974E-B211F8F378EA";
        static void Main(string[] args)
        {
            string DeviceID = GetDeviceID("Client04");
            if (DeviceID != null)
            {
                string status = Jobqueue(DeviceID, false);
                Console.WriteLine(status);
            }
            else
            {
                Console.WriteLine("Device not found!");
            }
            Console.ReadKey();
        }

        // Function for retrieving jobqueue of device
        private static string Jobqueue(string deviceID, Boolean bShowAll)
        {
            /* Retrieve Jobqueue of the device
               Endpoint: "/jobqueue/{deviceID}/{bShowAll}
                 Method: GET
                 Path parameters:
                    deviceID: Id of the device
                    bShowAll: boolean
                 Authentication:
                     Basic authentication
                 Headers:
                     ApiKey: “Your Api-Key”
             */

            string URL = baseurl + "/jobqueue/" + deviceID + "/" + bShowAll;
            var client = new RestClient(URL);
            //  Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            //  ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            //  Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.GET);
            // Execute method
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<JObject> jsonResponse = JsonConvert.DeserializeObject<List<JObject>>(response.Content);
                if ((jsonResponse != null) && (jsonResponse.Any()))
                {
                    foreach (var status in jsonResponse)
                    {
                        if (status.GetValue("Status").ToString() == "ERROR" || status.GetValue("Status").ToString() == "FAILED" || status.GetValue("Status").ToString() == "SCHEDULED")
                        {
                            string rowID = status.GetValue("RowId").ToString();
                            string jobID = status.GetValue("JobID").ToString();
                            Console.WriteLine(ReapplyJob(deviceID, jobID, rowID));
                        }
                    }
                    return "Success";
                }
            }
            return "Failed";
        }

        // Function to re-apply all the pending jobs
        private static string ReapplyJob(string deviceID, string jobID, string rowID)
        {
            /* Reapply job
               Endpoint: "/jobqueue/{jobID}/{deviceID}/{rowID};
                 Method: PUT
                 Path parameters:
                    jobID: ID of the job to apply
                    deviceID: Id of the device
                    rowID: Jobqueue ID of the job
                 Authentication:
                     Basic authentication
                 Headers:
                     ApiKey: “Your Api-Key”
             */

            string URL = baseurl + "/jobqueue/" + jobID + "/" + deviceID + "/" + rowID;
            var client = new RestClient(URL);
            //  Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            //  ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            //  Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.PUT);
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
