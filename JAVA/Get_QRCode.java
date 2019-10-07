import java.io.FileOutputStream;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import org.json.JSONArray;
import org.json.JSONObject;
import okhttp3.Credentials;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;

public class Get_QRCode
{
	public static String baseURL = "http://suremdm.42gears.com"; // BaseURL of SureMDM
	private static String username = "Username";
	private static String password = "Password";
	private static String apikey = "Your ApiKey";

	public static void main(String[] args) throws Exception
	{
		// Get base64 QRcode
		String base64String = GetQRCode("Group Name").replace("\"", "");

		if (base64String != null)
		{
			// Store QR code image in file
			byte[] imageFile = Base64.getDecoder().decode(base64String);
			try (OutputStream stream = new FileOutputStream("File path"))  //File path i.e:D:\\QRcode.png
			{
				stream.write(imageFile);
			}
		}
		else
		{
			System.out.println("Invalid request!");
		}
	}

	private static String GetQRCode(String groupName) throws Exception
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
		String URL = baseURL + "/QRCode/" + GetGroupID(groupName) + "/default/true/UseSystemGenerated";
		// Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL)
				// Send payload
				.get()
				// Basic authentication header
				.addHeader("Authorization", Credentials.basic(username, password))
				// ApiKey Header
				.addHeader("ApiKey", apikey)
				// Set content type
				.addHeader("Content-Type", "application/json").build();
		// Execute request
		Response response = client.newCall(request).execute();
		if (response.isSuccessful())
		{
			return response.body().string();
		}
		return null;
	}

	private static String GetGroupID(String groupName) throws Exception
	{
		// For home group no need to get groupID
		if (groupName.equalsIgnoreCase("Home"))
		{
			return groupName;
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
		String URL = "https://suremdm.42gears.com/api/group/1/getall";
		// Create request
		OkHttpClient client = new OkHttpClient();
		Request request = new Request.Builder().url(URL).get()
				// Basic authentication header
				.addHeader("Authorization", Credentials.basic(username, password))
				// apikey Header
				.addHeader("apikey", "37A007A7-5D12-4D14-974E-B211F8F378EA")
				// Set content type
				.addHeader("Content-Type", "application/json").build();
		// Execute request
		Response response = client.newCall(request).execute();
		// Extracting GroupID
		if (response.isSuccessful())
		{
			String data = response.body().string();
			JSONObject jsonObj = new JSONObject(data);
			JSONArray groups = jsonObj.getJSONArray("Groups");
			for (int index = 0; index < groups.length(); index++)
			{
				JSONObject group = groups.getJSONObject(index);
				if (group.get("GroupName").equals(groupName))
				{
					return group.get("GroupID").toString();
				}
			}
		}
		return null;
	}
}
