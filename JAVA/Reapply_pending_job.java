import java.io.IOException;

import org.json.JSONArray;
import org.json.JSONObject;

import okhttp3.Credentials;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class Reapply_pending_job
{
	public static String baseurl = "https://suremdm.42gears.com/api";  // BaseURL of SureMDM
	private static String Username = "Username";
	private static String Password = "Password";
	private static String ApiKey = "Your Apikey";

	public static void main(String[] args) throws Exception
	{
		String DeviceID = GetDeviceID("Device_Name");
		if (DeviceID != null)
		{
			String status = Jobqueue(DeviceID, false);
			System.out.print(status);
		}
		else
		{
			System.out.print("Device not found!");
		}
	}

	private static void ReapplyPendingJobs(String jobID, String deviceID, String rowID) throws Exception
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
		      ApiKey: �Your Api-Key�
		*/

		//SureMDM API URL of your account
		String URL = baseurl + "/jobqueue/" + jobID + "/" + deviceID + "/" + rowID;
		// Empty body
		RequestBody body = RequestBody.create(null, new byte[] {});
		//Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL)
				//Send payload
				.put(body)
				//Basic authentication header
				.addHeader("Authorization", Credentials.basic(Username, Password))
				//ApiKey header
				.addHeader("ApiKey", ApiKey)
				//Set content type
				.addHeader("Content-Type", "application/json").build();
		//Send request
		Response response = client.newCall(request).execute();
		System.out.println(response.body().string());
	}

	private static String Jobqueue(String deviceID, Boolean bShowAll) throws Exception
	{
		/* Retrieve Jobqueue of the device
		Endpoint: "/jobqueue/{deviceID}/{bShowAll}
		  Method: POST
		  Path parameters:
		     deviceID: Id of the device
		     bShowAll: boolean
		  Authentication:
		      Basic authentication
		  Headers:
		      ApiKey: �Your Api-Key�
		*/

		// API URL
		String URL = baseurl + "/jobqueue/" + deviceID + "/" + bShowAll;
		// Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL)
				// Send payload
				.get()
				// Basic authentication header
				.addHeader("Authorization", Credentials.basic(Username, Password))
				// ApiKey Header
				.addHeader("ApiKey", ApiKey)
				// Set content type
				.addHeader("Content-Type", "application/json").build();
		// Execute request
		Response response = client.newCall(request).execute();
		String data = response.body().string();
		if (response.isSuccessful())
		{
			if (data != "[]")
			{
				JSONArray ja = new JSONArray(data);
				for (int i = 0; i < ja.length(); i++)
				{
					JSONObject jo = ja.getJSONObject(i);
					if (jo.get("Status").equals("ERROR") || jo.get("Status").equals("FAILED")
							|| jo.get("Status").equals("SCHEDULED"))
					{
						String rowID = jo.getString("RowId");
						String jobID = jo.getString("JobID");
						ReapplyPendingJobs(jobID, deviceID, rowID);
					}
				}
				return "Success";
			}
		}
		return "Failed";
	}

	private static String GetDeviceID(String deviceName) throws Exception
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
		    ApiKey: �Your Api-Key� 
		*/

		// request body
		JSONObject PayLoad = new JSONObject();
		PayLoad.put("ID", "AllDevices");
		PayLoad.put("IsSearch", true);
		PayLoad.put("Limit", 20);
		PayLoad.put("SearchColumns", new JSONArray("[\"DeviceName\"]"));
		PayLoad.put("SearchValue", deviceName);
		PayLoad.put("SortColumn", "LastTimeStamp");
		PayLoad.put("SortOrder", "asc");
		MediaType mediaType = MediaType.parse("application/json");
		RequestBody body = RequestBody.create(mediaType, PayLoad.toString());

		// API URL
		String URL = baseurl + "/device";
		// Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL)
				// Send payload
				.post(body)
				// Basic authentication header
				.addHeader("Authorization", Credentials.basic(Username, Password))
				// ApiKey Header
				.addHeader("ApiKey", ApiKey)
				// Set content type
				.addHeader("Content-Type", "application/json").build();
		// Execute request
		Response response = client.newCall(request).execute();
		// Extracting DeviceID
		String data = response.body().string();
		if (response.isSuccessful())
		{
			JSONObject jsonObj = new JSONObject(data);
			JSONArray devices = jsonObj.getJSONArray("rows");
			for (int index = 0; index < devices.length(); index++)
			{
				JSONObject device = devices.getJSONObject(index);
				if (device.get("DeviceName").equals(deviceName))
				{
					return device.get("DeviceID").toString();
				}
			}
		}
		return null;
	}
}
