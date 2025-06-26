#!/bin/bash

cd /opt/TOMs/IoTCenter/bin
nohup dotnet GWHost1.dll >/dev/null 2>&1 &
echo "start host"

sleep 5

cd /opt/TOMs/IoTCenter/IoTCenterWeb/publish
nohup dotnet IoTCenterWebApi.dll >/dev/null 2>&1 &
echo "start webApi"

wait