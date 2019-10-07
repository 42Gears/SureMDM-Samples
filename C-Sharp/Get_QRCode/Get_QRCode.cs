/*
Code for generating QR code to enroll device in particular group on SureMDM account.
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.IO;
using System.Net;

namespace Get_QRCode
{
    class Get_QRCode
    {
        public static string baseurl = "http://suremdm.42gears.com"; // BaseURL of SureMDM
        private static string Username = "Username";
        private static string Password = "Password";
        private static string ApiKey = "Your ApiKey";
        static void Main(string[] args)
        {
            //Get base64 QRcode
            string base64String = GetQRCode("Group Name");

            base64String = base64String.Replace("\"","");
            Console.WriteLine(base64String);Console.WriteLine("hello world");
            if (base64String != null)
            {
                Byte[] QRCode = Convert.FromBase64String(base64String);
                using (var imageFile = new FileStream("File path", FileMode.Create)) //File path i.e: "D:\\QRcode.png"
                {
                    imageFile.Write(QRCode, 0, QRCode.Length);
                    imageFile.Flush();
                }
            }
            else
            {
                Console.WriteLine("Invalid request!");
            }
            Console.ReadKey();
        }

        private static string GetQRCode(string groupName)
        {
            /*
            Retreiving QRCode for enrolling device in particular group
                Endpoint: /QRCode/{GroupID}/default/true/UseSystemGenerated
                Method: GET
                Path Params:
                    GroupID: ID of the group 
                Authentication:
                    Basic authentication
                Headers:
                    ApiKey: “Your Api-Key”
		    */

            // API URL
            string URL = baseurl + "/QRCode/" + GetGroupID(groupName) + "/default/true/UseSystemGenerated";
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

            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(response.Content))
            {
                return response.Content;
            }
            return null;
        }

        static string GetGroupID(string GroupName)
        {
            // For home group no need to get groupID
            if (string.Equals(GroupName,"Home",StringComparison.InvariantCultureIgnoreCase))
            {
                return GroupName;
            }

            /*
            Retreiving group ID
                Endpoint: /group/1/getall
                Method: GET
                Params: NA
                Authentication:
                    Basic authentication
                Headers:
                    ApiKey: “Your Api-Key” 
            */
            // API URL
            string URL = "https://suremdm.42gears.com/api/group/1/getall";
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
                var OutPut = JsonConvert.DeserializeObject<JObject>(response.Content);
                foreach (var group in OutPut["Groups"])
                {
                    if ((string)group["GroupName"] == GroupName)
                    {
                        Console.WriteLine("hello " + group["GroupID"].ToString());
                        return group["GroupID"].ToString();
                    }
                }
            }
            return null;
        }
    }
}
