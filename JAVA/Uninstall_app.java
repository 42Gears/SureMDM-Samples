
import java.util.Base64;
import org.json.JSONArray;
import org.json.JSONObject;
import okhttp3.Credentials;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class Uninstall_app
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
			String status = UninstallApplication(DeviceID, "Application_name");
			//String status = GetAppID(DeviceID, "Ludo King");
			System.out.print(status);
		}
		else
		{
			System.out.print("Device not found!");
		}
	}

	private static String UninstallApplication(String deviceID, String appName) throws Exception
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
		      ApiKey: �Your Api-Key�
		*/
		
		// Create job specific PayLoad
		JSONObject PayLoad = new JSONObject();
		PayLoad.put("AppIds", new JSONArray("[" + GetAppID(deviceID, appName) + "]"));
		// convert payload to base64 string
		String PayLoadBase64=Base64.getEncoder().encodeToString(PayLoad.toString().getBytes());
		
		// Request payload for uninstalling the app
		JSONObject RequestPayLoad= new JSONObject();
		RequestPayLoad.put("JobType", "UNINSTALL_APPLICATION");
		RequestPayLoad.put("DeviceID",deviceID );
		RequestPayLoad.put("PayLoad",PayLoadBase64);
		MediaType mediaType = MediaType.parse("application/json");
		RequestBody body = RequestBody.create(mediaType, RequestPayLoad.toString());

		// API URL
		String URL = baseurl + "/dynamicjob";
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
	
	private static String GetAppID(String deviceID, String appName) throws Exception
	{
		// API URL
		String URL = baseurl + "/installedapp/android/" + deviceID + "/device";
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
					if (jo.get("Name").equals(appName))
					{
						return jo.get("Id").toString();
					}
				}
			}
		}
		return null;
	}


	private static String RefreshDeviceApps(String deviceID) throws Exception
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
		      ApiKey: �Your Api-Key�
		*/

		// ... API URL
		String URL = baseurl + "/dynamicjob";
		// ... PayLoad data
		JSONObject PayLoad = new JSONObject();
		PayLoad.put("DeviceID", deviceID);
		PayLoad.put("JobType", "GET_DEVICE_APPS");
		// ... request body
		MediaType mediaType = MediaType.parse("application/json");
		RequestBody body = RequestBody.create(mediaType, PayLoad.toString());
		// ... Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL)
				// ... Send payload
				.post(body)
				// ... Basic authentication header
				.addHeader("Authorization", Credentials.basic(Username, Password))
				// ... ApiKey Header
				.addHeader("ApiKey", ApiKey)
				// ... Set content type
				.addHeader("Content-Type", "application/json").build();
		// ... Execute request
		Response response = client.newCall(request).execute();

		return response.body().string();
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
