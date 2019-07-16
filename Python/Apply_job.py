import requests,json

baseurl="https://suremdm.42gears.com/api"
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
    url = baseurl+"/device"
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

# Method for getting ID of the folder
def GetFolderID(folderName):
    '''
    Retrieve ID of the folder
        Endpoint: /jobfolder/all
        Method: GET
        Parameters: NA
        Authentication:
            Basic authentication
        Headers:
            ApiKey: “Your Api-Key”
    '''
    # Api url
    url = baseurl+"/jobfolder/all"
    # Headers
    headers = {
        # Api-Key header
        'ApiKey': ApiKey,
        # Set Content type
        'Content-Type': "application/json",
        }
    # Basic authentication credentials
    Credentials=(Username,Password)
    # Executing request
    response = requests.get(url,auth=Credentials,headers=headers)
    # Extracting required FolderID
    if response.status_code == 200:
        if response.text != '[]':
            data = response.json()
            for folder in data:
                if folder['FolderName'] == folderName:
                    return folder['FolderID']
            return 'null'
    else:
        return "null"

# Method for getting ID of the job
def GetJobID(jobName,folderID):
    '''
    Retrieve ID of the job
        Endpoint: /job
        Method: GET
        Query Parameters: 
            FolderID: ID of the job folder
        Authentication:
            Basic authentication
        Headers:
            ApiKey: “Your Api-Key”
    '''
    # Api url
    url = baseurl+"/job"
    # Headers
    headers = {
        # Api-Key header
        'ApiKey': ApiKey,
        # Set Content type
        'Content-Type': "application/json",
        }
    # Basic authentication credentials
    Credentials=(Username,Password)
    # Query parameters
    params={"FolderID":folderID}
    # Executing request
    response = requests.get(url,auth=Credentials,params=params,headers=headers)
    # Extracting required JobID
    if response.status_code == 200:
        if response.text != '[]':
            data = response.json()
            for job in data:
                if job['JobName'] == jobName:
                    return job['JobID']
    else:
        return None

# Method for applying job
def ApplyJob(jobName,folderName,deviceID):
    '''
    Apply job on device
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
    '''
    if folderName!=None:
        FolderID=GetFolderID(folderName)
    else:
        FolderID='null'
    JobID=GetJobID(jobName,FolderID)
    PayLoad={
        "DeviceIds": [deviceID],
        "JobId": JobID
    }
    # Api url
    url = baseurl+"/jobassignment"
    # Headers
    headers = {
        # Api-Key header
        'ApiKey': ApiKey,
        # Set Content type
        'Content-Type': "application/json",
        }
    # Basic authentication credentials
    Credentials=(Username,Password)
    # Executing request
    response = requests.post(url,auth=Credentials,json=PayLoad,headers=headers)
    return response.text

# Main
DeviceID=GetDeviceID("Device_Name")
if DeviceID!=None:
    status=ApplyJob("Job_Name","Folder_Name",DeviceID)
    print(status)
else:
    print('Device not found!')