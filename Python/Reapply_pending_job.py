# This program re-applies all pending jobs from device job queue
import requests,json

baseurl="https://suremdm.42gears.com/api"  # BaseURL of SureMDM
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
            for device in data['rows']:
                if device['DeviceName'] == deviceName:
                    return device["DeviceID"]
    else:
        return None


def ReapplyJob(deviceID,jobID,rowID):
    # Api url
    url = baseurl + "/jobqueue/" + jobID + "/" + deviceID + "/" + rowID
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
    response = requests.put(url,auth=Credentials,headers=headers)
    print(response.text)
    

def JobQueue(deviceID,bShowAll):
    '''
    Retrieve Jobqueue of the device
        Endpoint: "/jobqueue/{deviceID}/{bShowAll}
            Method: POST
            Path parameters:
            deviceID: Id of the device
            bShowAll: boolean
            Authentication:
                Basic authentication
            Headers:
                ApiKey: “Your Api-Key”
    '''
    # Api url
    url = baseurl + "/jobqueue/" + deviceID + "/" + str(bShowAll)
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
    if response.status_code == 200:
        if response.text != '[]':
            data = response.json()
            for job in data:
                if job['Status'] == "ERROR" or job['Status'] == "FAILED" or job['Status']== "SCHEDULED":
                    rowID=job['RowId']
                    jobID=job['JobID']
                    ReapplyJob(deviceID,jobID,rowID)
                    return 'Success'
    else:
        return 'Failed'

# Main
DeviceID=GetDeviceID("Device_Name")
if DeviceID!=None:
    status=JobQueue(DeviceID,False)
    print(status)
else:
    print('Device not found')