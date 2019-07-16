using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Apply_job
{
    class Apply_job
    {
        public static string baseurl = "https://suremdm.42gears.com/api";
        private static string Username = "Username";
        private static string Password = "Password";
        private static string ApiKey = "Your ApiKey";
        static void Main(string[] args)
        {
            // Applying job on the device requires ID of the Device and ID of the job
            string DeviceID = GetDeviceID("Device_Name");
            if (DeviceID != null)
            {
                ApplyJob("Job_Name", "Jobfolder name", DeviceID);
            }
            else
            {
                Console.WriteLine("Device not found");
            }

            Console.ReadKey();
        }

        // Function to apply job
        private static void ApplyJob(string JobName, string FolderName, string deviceID)
        {
            // Retrieve ID of the jobfolder 
            string FolderID;
            if (FolderName != null)
            {
                FolderID = GetFolderID(FolderName);
            }
            else
            {
                FolderID = null;
            }

            // Pay load for applying job
            var JobApplication = new
            {
                DeviceIds = new List<string>() { deviceID },
                JobId = GetJobID(JobName, FolderID)
            };

            /* Apply job on device
                Endpoint: /jobassignment
                Method: POST
                Request Body: 
                    {
                        "DeviceIds": string[],
                        "JobId": string
                    }
                Authentication:
                    Basic authentication
                Headers:
                    ApiKey: “Your Api-Key”
            */

            string URL = baseurl + "/jobassignment";
            var client = new RestClient(URL);
            //  Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            //  ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            //  Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(JobApplication);

            // Execute request
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (Convert.ToBoolean(response.Content.ToString()))
                {
                    Console.WriteLine("Job applied successfully.");
                }
                else
                {
                    Console.WriteLine("Job application failed!");
                }
            }
            else
            {
                Console.WriteLine(response.Content.ToString());
            }
        }

        // Function to retrieve ID of the job
        private static object GetJobID(string jobName, string folderid)
        {
            /* Retrieve ID of the job
                Endpoint: /job
                Method: GET
                Query Parameters: 
                    FolderID: ID of the job folder
                Authentication:
                    Basic authentication
                Headers:
                    ApiKey: “Your Api-Key”
            */

            string URL = baseurl + "/job";
            var client = new RestClient(URL);
            //  Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            //  ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            //  Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.GET);
            request.AddParameter("FolderID", folderid, ParameterType.QueryString);
            //  Execute request
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var OutPut = JsonConvert.DeserializeObject(response.Content);
                var jsonResponse = JsonConvert.DeserializeObject<List<JObject>>(response.Content);
                if ((jsonResponse != null) && (jsonResponse.Any()))
                {
                    foreach (var job in jsonResponse)
                    {
                        if (job.GetValue("JobName").ToString() == jobName)
                        {
                            return job.GetValue("JobID").ToString();
                        }
                    }
                }
            }
            return null;
        }

        // Function to retrieve ID of the jobfolder
        private static string GetFolderID(string folderName)
        {
            /* Retrieve ID of the folder
                Endpoint: /jobfolder/all
                Method: GET
                Parameters: NA
                Authentication:
                    Basic authentication
                Headers:
                    ApiKey: “Your Api-Key”
            */

            string URL = baseurl + "/jobfolder/all";
            var client = new RestClient(URL);
            //  Basic authentication header
            client.Authenticator = new HttpBasicAuthenticator(Username, Password);
            //  ApiKey Header
            client.AddDefaultHeader("ApiKey", ApiKey);
            //  Set content type
            client.AddDefaultHeader("Content-Type", "application/json");
            // Set request method
            var request = new RestRequest(Method.GET);
            //  Execute request
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var OutPut = JsonConvert.DeserializeObject(response.Content);
                List<JObject> jsonResponse = JsonConvert.DeserializeObject<List<JObject>>(response.Content);
                if ((jsonResponse != null) && (jsonResponse.Any()))
                {
                    foreach (var folder in jsonResponse)
                    {
                        if (folder.GetValue("FolderName").ToString() == folderName)
                        {
                            return folder.GetValue("FolderID").ToString();
                        }
                    }
                }
            }
            return "null";
        }

        // Function to retrieve ID of the device
        private static string GetDeviceID(string deviceName)
        {
            /*  Retreiving information of device
                 Endpoint: /device
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
