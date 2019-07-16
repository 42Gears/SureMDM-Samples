import requests,json,base64

BaseURL="https://suremdm.42gears.com/api"
Username="Username"
Password="Password"
ApiKey="Your ApiKey"

# Method for getting device info
def GetDeviceID(deviceName):
    '''
    Retreiving information of device
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
    '''
    # Api url
    url = BaseURL+"/device"
    # Headers
    headers = {
        # Api-Key header
        'ApiKey': ApiKey,
        # Set Content type
        'Content-Type': "application/json",
        }
    # Basic authentication credentials
    Credentials=(Username,Password)
    # Request body
    PayLoad={
        "ID" : "AllDevices",
        "IsSearch" : bool(1),
        "Limit" : 10,
        "SearchColumns" : ["DeviceName"],
        "SearchValue" : deviceName,
        "SortColumn" : "LastTimeStamp",
        "SortOrder" : "asc"
    }
    # Executing request
    response = requests.post(url,auth=Credentials,json=PayLoad,headers=headers)
    # Extracting required GroupID
    if response.status_code == 200:
        if response.text != '[]':
            data = response.json()
            for group in data['rows']:
                if group['DeviceName'] == deviceName:
                    return group["DeviceID"]
    else:
        return None

def GetAppID(deviceID,appName):
    """
    Retrieve AppID (Package name) of a app
        Endpoint: /installedapp/android/{DeviceId}/device
        Method: GET
        Path parameters:
            DeviceID: ID of the device allocated by the SureMDM.
        Authentication:
            Basic authentication
        Headers:
            ApiKey: “Your Api-Key”
    """
    url = BaseURL+"/installedapp/android/"+deviceID+"/device"
    #  Headers
    headers = {
        #  Api-Key header
        'ApiKey': ApiKey,
        #  Set Content type
        'Content-Type': "application/json",
        }
    #  Basic authentication credentials
    Credentials=(Username,Password)
    #  send request
    response = requests.get(url,auth=Credentials,headers=headers)
    # Extracting required GroupID
    if response.status_code == 200:
        if response.text != '[]':
            jsonResponse = response.json()
            for app in jsonResponse:
                if app['Name'] == appName:
                    return app['Id']
    else:
        return None

def UninstallApp(deviceID,appName):
    """
    Retrieve AppID (Package name) of a app
        Endpoint: /dynamicjob
        Method: POST
        Request Body:
        Type: application/json
        Body parameters:
            DeviceID: ID of the device allocated by the SureMDM.
            JobType: Job type
            PayLoad: Job specific payload
        Authentication:
            Basic authentication
        Headers:
            ApiKey: “Your Api-Key”
    """
    url = BaseURL+"/dynamicjob"
    #  Headers
    headers = {
        #  Api-Key header
        'ApiKey': ApiKey,
        #  Set Content type
        'Content-Type': "application/json",
        }
    #  Basic authentication credentials
    Credentials=(Username,Password)
    # Create job specific PayLoad
    PayLoad={
        "AppIds":[GetAppID(deviceID,appName)]
    }
    # Convert payload to base64 string
    PayLoadStr=json.dumps(PayLoad)
    PayLoadBase64=base64.b64encode(PayLoadStr.encode('utf-8'))
    # Request payload for refreshing device
    RequestPayLoad={
        "JobType": "UNINSTALL_APPLICATION",
        "DeviceID": deviceID,
        "PayLoad":PayLoadBase64.decode('utf-8')
    }
    #  send request
    response = requests.post(url,auth=Credentials,json=RequestPayLoad,headers=headers)
    return response.text

# Retrieve Device ID
DeviceID=GetDeviceID("Device_Name")
if DeviceID!=None:
    # Uninstall app
    status=UninstallApp(DeviceID,"Application_Name")
    print(status)
else:
    print('Device not found!')