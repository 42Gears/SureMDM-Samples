import requests,json,base64

baseurl = "http://suremdm.42gears.com" # BaseURL of SureMDM
Username="Username"
Password="Password"
ApiKey="Your ApiKey"

def GetGroupID(groupName):
    # For home group no need to get groupID
    if groupName.casefold() == "Home":  
        return groupName

    '''
    Retreiving group ID
        Endpoint: /group/1/getall
        Method: GET
        Params: NA
        Authentication:
            Basic authentication
        Headers:
            ApiKey: “Your Api-Key”
    '''
    # Api url
    url = baseurl+"/group/1/getall"
    # Add headers
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
    # Extracting required GroupID
    if response.status_code == 200:
        data = response.json()
        for group in data['Groups']:
            if group['GroupName'] == groupName:
                return group["GroupID"]
    else:
        return None

def GetQRCode(groupName):
    GroupID=GetGroupID(groupName)
    '''
    Retreiving QRCode for enrolling device in particular group
        Endpoint: /QRCode/{GroupID}/default/true/UseSystemGenerated
        Method: GET
        Path Params:
            GroupID: ID of the group 
        Authentication:
            Basic authentication
        Headers:
            ApiKey: “Your Api-Key”
    '''
    # Api url
    url = baseurl + "/QRCode/" + str(GroupID) + "/default/true/UseSystemGenerated"
    # Add headers
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
    # Extracting required GroupID
    if response.status_code == 200:
        return response.text
    else:
        return None


# Main starts
# Get base64 QRcode
base64String=GetQRCode("Group Name")
base64String=base64String.replace("\"","")
if base64String!=None:
    with open("File Path", "wb") as stream:        # File path i.e:D:\\QRcode.png
        stream.write(base64.b64decode(base64String))
else:
    print("Invalid request!")

