import org.json.JSONArray;
import org.json.JSONObject;
import okhttp3.Credentials;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class Remote_support
{
	public static String baseurl = "https://suremdm.42gears.com/api";  // BaseURL of SureMDM
	private static String Username = "Username";
	private static String Password = "Password";
	private static String ApiKey = "Your ApiKey";

	public static void main(String[] args) throws Exception
	{
		String RemoteURL = GetRemoteSupportURL("lenovo rj");
		System.out.println(RemoteURL);
	}

	private static String GetRemoteSupportURL(String deviceName) throws Exception
	{
		String DeviceID = GetDeviceID(deviceName);
		// API URL
		String URL = baseurl + "/device/" + DeviceID;

		/*  Retreiving Id, name, uerID, pltFrmType, agentversion of the device
		Endpoint: /api/device/{DeviceID}:
		Method: GET
		Authentication:
		    Basic authentication
		Headers:
		    ApiKey: �Your Api-Key� 
		*/

		// Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL).get()
				// Basic authentication header
				.addHeader("Authorization", Credentials.basic(Username, Password))
				// ApiKey Header
				.addHeader("ApiKey", ApiKey)
				// Set content type
				.addHeader("Content-Type", "application/json")

				.build();
		// Execute request
		Response response = client.newCall(request).execute();
		// Extracting DeviceID
		String data = response.body().string();
		if (response.isSuccessful())
		{
			if (data != "[]")
			{
				JSONArray ja = new JSONArray(data);
				for (int i = 0; i < ja.length(); i++)
				{
					JSONObject jo = ja.getJSONObject(i);
					if (jo.get("DeviceName").equals(deviceName))
					{
						String remoteSupporturl = "https://suremdm.42gears.com" + "/RemoteSupport.aspx?" + "id="
								+ jo.get("DeviceID").toString() + "&name=" + jo.get("DeviceName").toString()
								+ "&userid=" + jo.get("UserID").toString() + "&pltFrmType="
								+ jo.get("PlatformType").toString() + "&agentversion="
								+ jo.get("AgentVersion").toString() + "&perm=126,127,128,129";

						return remoteSupporturl;
					}
				}
			}
		}
		return null;
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
