
# DC-WebUI

This repository contains source code for Web UI used by data collection system. This application will allow users to authenticate and submit Ilr files.This application works in conjuction with a running service fabric instance which will be resposible for processing the files.

## First time setup
Run the setup.bat from root folder and this will setup the pre commit hook. Pre commti hook will use the powershell script to check if any sensitive data is being pushed into the repo and will disallow the commit.

## User Settings
If you have a configuration file setup inside private repo, pre-build event will copy the file (appsettings.**YourWindowsUsername**.json) from your name's relevent folder into the application and this file will be used for local development, otherwise default settings will be used for appsettings.jscon

## Runtime
Source code is based on .Net core 2.0.

## Authentication
Web UI relies on Information Management Services (IDAMS) for authentication, however this can be disabled for local testing by setting the "Enabled" flag to false in "AuthenticationSettings" of appsettings.json file. If disabled application will use 

## Requirements
* Working service fabric insatnce configured to recieve messages from a queue configured in the appsettings.json file "CloudStorageSettings".
* For writing debug logs and errors, connection string pointing to an empty database mentioned in the "Applogs" section in connection strings.
* Working  web api instance to receive job request. Code will be available in git hub for the webapi project in github.
* If authentication is enabled, for local development you can use the DC identity server available from VSTS repo.


## App Settings
```
{
  "AuthenticationSettings": {
    "Enabled": "true", * For local development this can be set to enabled false and authentication will be skipped.
    "WtRealm": "",
    "MetadataAddress": "https://adfs.preprod.skillsfunding.service.gov.uk/FederationMetadata/2007-06/FederationMetadata.xml"
  },
  "CloudStorageSettings": {
    "ConnectionString": "",
    "ContainerName": ""
  },
  "ConnectionStrings": {
    "AppLogs": "",
    "Permissions": ""

  },
  "JobQueueApiSettings": {
    "BaseUrl": "http://localhost:2088/api"
  }
}
```

