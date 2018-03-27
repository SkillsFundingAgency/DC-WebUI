# DC-WebUI

This repository contains source code for Web UI used by data collection system. This application will allow users to authenticate and submit Ilr files.This application works in conjuction with a running service fabric instance which will be resposible for processing the files.

## First time setup
Run the setup.bat from root folder and this will setup the pre commit hook. Pre commti hook will use the powershell script to check if any sensitive data is being pushed into the repo and will disallow the commit.

## Runtime
Source code is based on .Net core 2.0.

## Authentication
Web UI relies on Information Management Services (IDAMS) for authentication, however this can be disabled for local testing by setting the "Enabled" flag to false in "AuthenticationSettings" of appsettings.json file. If disabled application will use 

## Requirements
1. Working service fabric insatnce configured to recieve messages from a queue configured in the appsettings.json file "ServiceBusQueueSettings" secton.
2. Cloud storage account configured and using the same container used by the service fabric instance and specified in the "CloudStorageSettings".
3. For writing debug logs and errors, connection string pointing to an empty database.


## App Settings
```
{
  "AuthenticationSettings": {
    "Enabled": "true",
    "WtRealm": "https://localhost",
    "MetadataAddress": ""
  },
  "ServiceBusQueueSettings": {
    "Name": "",
    "ConnectionString": ""
  },
  "CloudStorageSettings": {
    "ConnectionString": "",
    "ContainerName": ""
  },
  "ConnectionStrings": {
    "AppLogs": ""
  }
```

