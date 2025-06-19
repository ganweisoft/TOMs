echo Start Install IoTCenter Service
echo Current Path %cd%
cd ..\..\
echo %cd%
set dir=D:\TOMs\IoTCenter\

@echo off
sc query IoTCenterDaemon |findstr /i "STATE">nul
if not errorlevel 1 (goto startDaemon) else goto createDaemon

:startDaemon
sc start "IoTCenterDaemon"

:createDaemon
sc create IoTCenterDaemon binPath= "\"%dir%dotnet\dotnet.exe\" \"%dir%services\daemon\IoTCenterWeb.Daemon.dll\"" DisplayName= "IoTCenterDaemon" start= auto  
sc start "IoTCenterDaemon"
echo IoTCenterDaemon Installed Successfully

@echo off
sc query IoTCenter |findstr /i "STATE">nul
if not errorlevel 1 (goto exist) else goto notexist 

:exist
sc start "IoTCenter"


:notexist
sc create IoTCenter binPath= "\"%dir%dotnet\dotnet.exe\" \"%dir%bin\GWHost1.dll\"" DisplayName= "IoTCenter Service" start= auto  
sc start "IoTCenter"
echo IoTCenter Installed Successfully

pause