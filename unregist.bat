echo Start UnInstalled  IoTCenter Service
echo Current Path %cd%
cd ..\..\
echo %cd%
taskkill /IM dotnet
set dir=D:\TOMs\IoTCenter\
echo --------------------

sc stop IoTCenterDaemon 
sc delete IoTCenterDaemon 
echo IoTCenterDaemon UnInstalled Successfully

sc stop IoTCenter 
sc delete  IoTCenter
echo IoTCenter UnInstalled Successfully

pause
