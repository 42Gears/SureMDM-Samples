import org.json.JSONArray;
import org.json.JSONObject;
import okhttp3.Credentials;
import okhttp3.HttpUrl;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class Apply_job
{
	public static String baseurl = "https://suremdm.42gears.com/api";  // BaseURL of SureMDM
	private static String Username = "Username";
	private static String Password = "Password";
	private static String ApiKey = "Your ApiKey";

	public static void main(String[] args) throws Exception
	{
		String DeviceID = GetDeviceID("Device_Name");
		if (DeviceID != null)
		{
			String status = Applyjob("Job_Name", null, DeviceID);
			System.out.print(status);
		}
		else
		{
			System.out.print("Device not found!");
		}
	}

	private static String Applyjob(String jobName, String folderName, String deviceID) throws Exception
	{
		String folderid;
		if (folderName != null)
		{
			folderid = GetFolderID(folderName);
		}
		else
		{
			folderid = "null";
		}

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
		    ApiKey: �Your Api-Key�
		*/

		// request body
		String jobid = GetJobID(jobName, folderid);
		// PayLoad data
		JSONObject PayLoad = new JSONObject();
		PayLoad.put("DeviceIds", new JSONArray("[" + deviceID + "]"));
		PayLoad.put("JobId", jobid);
		MediaType mediaType = MediaType.parse("application/json");
		RequestBody body = RequestBody.create(mediaType, PayLoad.toString());

		// API URL
		String URL = baseurl + "/jobassignment";
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
		return response.body().string();
	}

	private static String GetJobID(String jobName, String folderID) throws Exception
	{
		/* Retrieve ID of the job
		Endpoint: /job
		Method: GET
		Query Parameters: 
		    FolderID: ID of the job folder
		Authentication:
		    Basic authentication
		Headers:
		    ApiKey: �Your Api-Key�
		*/

		// API URL
		String URL = baseurl + "/job";
		// Query parameters
		HttpUrl.Builder httpBuider = HttpUrl.parse(URL).newBuilder();
		httpBuider.addQueryParameter("FolderID", folderID);
		URL = httpBuider.build().toString();
		// Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL).get()
				// Basic authentication header
				.addHeader("Authorization", Credentials.basic(Username, Password))
				// ApiKey Header
				.addHeader("ApiKey", ApiKey)
				// Set content type
				.addHeader("Content-Type", "application/json").build();
		// Execute request
		Response response = client.newCall(request).execute();
		// System.out.print(response.body().string());
		// Extracting folderid
		String data = response.body().string();
		if (response.isSuccessful())
		{
			if (data != "[]")
			{
				JSONArray jobs = new JSONArray(data);
				for (int i = 0; i < jobs.length(); i++)
				{
					JSONObject job = jobs.getJSONObject(i);
					if (job.get("JobName").equals(jobName))
					{
						return job.get("JobID").toString();
					}
				}

			}
		}
		return null;
	}

	private static String GetFolderID(String folderName) throws Exception
	{
		/* Retrieve ID of the folder
		Endpoint: /jobfolder/all
		Method: GET
		Parameters: NA
		Authentication:
		    Basic authentication
		Headers:
		    ApiKey: �Your Api-Key�
		*/

		// API URL
		String URL = baseurl + "/jobfolder/all";
		// Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL).get()
				// Basic authentication header
				.addHeader("Authorization", Credentials.basic(Username, Password))
				// ApiKey Header
				.addHeader("ApiKey", ApiKey)
				// Set content type
				.addHeader("Content-Type", "application/json").build();
		// Execute request
		Response response = client.newCall(request).execute();
		// Extracting folderid
		String data = response.body().string();
		if (response.isSuccessful())
		{
			if (data != "[]")
			{
				JSONArray ja = new JSONArray(data);
				for (int i = 0; i < ja.length(); i++)
				{
					JSONObject jo = ja.getJSONObject(i);
					if (jo.get("FolderName").equals(folderName))
					{
						return jo.get("FolderID").toString();
					}
				}

			}
		}
		return "null";
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
