@echo off  
setlocal enabledelelayedexpansion  

rem Switch to the directory where the current batch file is located  
cd /d "%~dp0%"  

chcp 65001 >nul  

rem Set build log directory  
set "logs_dir=build_logs.txt"  
del /q /f %logs_dir%  
call :GetTimestamp  
echo [!formatted_datetime!] Deleted generated build log file >>%logs_dir%  
rem Set actual release directory  
set "release_dir=.\Release"  
call :GetTimestamp  
echo [!formatted_datetime!] Set overall release directory path: %release_dir% >>%logs_dir%  
rem Set gateway release directory  
set "gateway_dir=%release_dir%\bin"  
call :GetTimestamp  
echo [!formatted_datetime!] Set gateway release directory path: %gateway_dir% >>%logs_dir%  
rem Set WebAPI release directory  
set "webapi_dir=%release_dir%\IoTCenterWeb\publish"  
call :GetTimestamp  
echo [!formatted_datetime!] Set webapi release directory path: %webapi_dir% >>%logs_dir%  
rem Set frontend release directory  
set "wwwroot_dir=%webapi_dir%\wwwroot"  
rem Set nvm installer path  
set "nvm_dir=subrepos\WebPlugins\src\front-end\nvm-setup.exe"  
call :GetTimestamp  
echo [!formatted_datetime!] Set nvm offline installer path: %nvm_dir% >>%logs_dir%  
rem Create release directory and clear old directory  
if exist "%release_dir%" (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Release directory^(%release_dir%^) already exists, deleting... >>%logs_dir%  
    rd /s /q "%release_dir%"  
    if errorlevel 1 (  
        call :GetTimestamp  
        echo [!formatted_datetime!] Failed to delete release directory! Files may be in use or insufficient permissions. >>%logs_dir%  
    )  
    call :GetTimestamp  
    echo [!formatted_datetime!] Release directory^(%release_dir%^) deleted... >>%logs_dir%  
    mkdir "%release_dir%"  
    call :GetTimestamp  
    echo [!formatted_datetime!] Release directory^(%release_dir%^) recreated... >>%logs_dir%  
) else (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Release directory^(%release_dir%^) does not exist, creating... >>%logs_dir%  
    mkdir "%release_dir%"  
    echo [!formatted_datetime!] Release directory^(%release_dir%^) created... >>%logs_dir%  
)  

rem Start frontend build  
rem ==============================  
rem Check and force nvm installation start  
rem ==============================  
:CheckNVMInstall  
if defined NVM_HOME (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Detected nvm is installed, installation path: %NVM_HOME% >>%logs_dir%  
    goto ContinueScript  
)  
if exist "%nvm_dir%" (  
    if defined NVM_HOME (  
        call :GetTimestamp  
        echo [!formatted_datetime!] Detected nvm is installed, installation path: %NVM_HOME% >>%logs_dir%  
    ) else (  
        call :GetTimestamp  
        echo [!formatted_datetime!] Installing nvm, please wait... >>%logs_dir%  
        call "%nvm_dir%"  
        goto ContinueScript  
    )  
) else (  
    echo [!formatted_datetime!] You may not have installed the nvm-setup program. Please manually install it in the directory (subrepos\WebPlugins\src\front-end\nvm-setup.exe) and try again. >>%logs_dir%  
    timeout /t 10 >nul  
    goto CheckNVMInstall  
)  
:ContinueScript  
rem Attempt to read NVM_HOME from user environment variables in the registry  
call :GetTimestamp  
echo [!formatted_datetime!] Refreshing %NVM_HOME% registry, retrieving nvm environment variables... >>%logs_dir%  
for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v NVM_HOME 2^>nul') do set "NVM_HOME=%%b"  
if defined NVM_HOME (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Completed nvm installation (%NVM_HOME%), continuing with subsequent operations... >>%logs_dir%  
) else (  
    goto CheckNVMInstall  
)  
rem ==============================  
rem Check and force nvm installation end  
rem ==============================  

set "NODE_VERSION1=14.21.3"  
set "NODE_VERSION2=23.1.0"  
call :GetTimestamp  
echo [!formatted_datetime!] Checking if Node.js %NVM_HOME%\v%NODE_VERSION1% is installed... >>%logs_dir%  
if exist "%NVM_HOME%\v%NODE_VERSION1%" (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Node.js v%NODE_VERSION1% is installed. >>%logs_dir%  
) else (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Node.js v%NODE_VERSION1% is not installed, installing... >>%logs_dir%  
    "%NVM_HOME%\nvm" install %NODE_VERSION1% --npm-site https://npm.taobao.org/mirrors/node  
    call :GetTimestamp  
    echo [!formatted_datetime!] Node.js v%NODE_VERSION1% installation completed... >>%logs_dir%  
)  
call :GetTimestamp  
echo [!formatted_datetime!] Checking if Node.js %NVM_HOME%\v%NODE_VERSION2% is installed... >>%logs_dir%  
if exist "%NVM_HOME%\v%NODE_VERSION2%" (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Node.js v%NODE_VERSION2% is installed. >>%logs_dir%  
) else (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Node.js v%NODE_VERSION2% is not installed, installing... >>%logs_dir%  
    "%NVM_HOME%\nvm" install %NODE_VERSION2% --npm-site https://npm.taobao.org/mirrors/node  
    echo [!formatted_datetime!] Node.js v%NODE_VERSION2% installation completed... >>%logs_dir%  
    "%NVM_HOME%\nvm" use %NODE_VERSION2%  
    "%NVM_HOME%\nvm" install pnpm -g  
    call :GetTimestamp  
    echo [!formatted_datetime!] Node.js v%NODE_VERSION2% pnpm installation completed... >>%logs_dir%  
)  
call :GetTimestamp  
echo [!formatted_datetime!] Starting frontend solution build... >>%logs_dir%  
rem Attempt to read NVM_SYMLINK from user environment variables in the registry  
for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v NVM_SYMLINK 2^>nul') do set "NVM_SYMLINK=%%b"  
call :GetTimestamp  
echo [!formatted_datetime!] Refreshing [%NODE_VERSION1%]^(%NVM_SYMLINK%^) registry, retrieving nodejs environment variables... >>%logs_dir%  
set "NODE_PATH=%NVM_SYMLINK%\node.exe"  
if defined NVM_SYMLINK (  
    "%NVM_HOME%\nvm" use %NODE_VERSION1%  
    PowerShell -Command "Test-Path '%NODE_PATH%'" | findstr /i "True" >nul  
    if %errorlevel% == 0 (  
        "%NVM_SYMLINK%\node" subrepos\WebPlugins\src\front-end\build.js  
        call :GetTimestamp  
        echo [!formatted_datetime!] Frontend plugins build completed... >>%logs_dir%  
    ) else (  
        call :GetTimestamp  
        echo [!formatted_datetime!] Node.js v%NODE_VERSION1% may have failed to install due to network issues... >>%logs_dir%  
        echo [!formatted_datetime!] Please manually uninstall %NVM_HOME%\unins000.exe and try again, or confirm if other command windows are open and causing conflicts. Close them and try again... >>%logs_dir%  
        pause  
        exit /b  
    )  
    "%NVM_HOME%\nvm" use %NODE_VERSION2%  
    rem Attempt to read NVM_SYMLINK from user environment variables in the registry  
    for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v NVM_SYMLINK 2^>nul') do set "NVM_SYMLINK=%%b"  
    call :GetTimestamp  
    echo [!formatted_datetime!] Refreshing [%NODE_VERSION2%]^(%NVM_SYMLINK%^) registry, retrieving nodejs environment variables... >>%logs_dir%  
    PowerShell -Command "Test-Path '%NODE_PATH%'" | findstr /i "True" >nul  
    if %errorlevel% == 0 (  
        "%NVM_SYMLINK%\node" subrepos\WebPlugins\src\front-end\build.js  
        call :GetTimestamp  
        echo [!formatted_datetime!] Scaffolding build completed... >>%logs_dir%  
    ) else (  
        call :GetTimestamp  
        echo [!formatted_datetime!] Node.js v%NODE_VERSION2% may have failed to install due to network issues... >>%logs_dir%  
        echo [!formatted_datetime!] Please manually uninstall %NVM_HOME%\unins000.exe and try again, or confirm if other command windows are open and causing conflicts. Close them and try again... >>%logs_dir%  
        pause  
        exit /b  
    )  
) else (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Node.js is not installed, frontend solution build failed... >>%logs_dir%  
    echo [!formatted_datetime!] Please manually uninstall %NVM_HOME%\unins000.exe and try again... >>%logs_dir%  
    pause  
    exit /b  
)  
call :GetTimestamp  
echo [!formatted_datetime!] Frontend solution build completed... >>%logs_dir%  
rem End frontend build  

rem Check if .NET SDK is installed  
call :GetTimestamp  
echo [!formatted_datetime!] Checking if .NET SDK is installed... >>%logs_dir%  
for /f "tokens=*" %%a in ('dotnet --list-sdks ^| sort /r') do (  
    for /f "tokens=2 delims=[]" %%b in ("%%a") do (  
        set "SDK_PATH=%%b"  
        goto :done  
    )  
)  
:done  
if not defined SDK_PATH (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Detected .NET SDK is not installed or installation path is not in global environment variables, please confirm... >>%logs_dir%  
    echo Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0  
    echo [!formatted_datetime!] Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%  
    pause  
    exit /b 1  
)  
if "%SDK_PATH%" == "" (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Detected .NET SDK is not installed or installation path is not in global environment variables, please confirm... >>%logs_dir%  
    echo Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0  
    echo [!formatted_datetime!] Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%  
    pause  
    exit /b 1  
)  
if exist "%SDK_PATH%\" (  
    call :GetTimestamp  
    echo [!formatted_datetime!] .NET SDK path "%SDK_PATH%" is installed. >>%logs_dir%  
) else (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Detected .NET SDK is not installed or installation path is not in global environment variables, please confirm... >>%logs_dir%  
    echo Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0  
    call :GetTimestamp  
    echo [!formatted_datetime!] Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%  
    pause  
    exit /b 1  
)  
rem Check if .NET SDK version 9.0 is installed  
for /f "tokens=*" %%a in ('dotnet --version') do set SDK_VERSION=%%a  
call :GetTimestamp  
echo [!formatted_datetime!] Current .NET SDK version: %SDK_VERSION% >>%logs_dir%  
echo %SDK_VERSION% | findstr "^9" >nul  
if %errorlevel% neq 0 (  
    echo Requires .NET SDK version 9.0  
    call :GetTimestamp  
    echo [!formatted_datetime!] Requires .NET SDK version 9.0 >>%logs_dir%  
    echo Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0  
    call :GetTimestamp  
    echo [!formatted_datetime!] Download link: https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%  
    pause  
    exit /b 1  
)  
rem Check if dotnet command is added to the environment variable PATH  
for %%A in ("%SDK_PATH%") do set "DOTNET_PATH=%%~dpA"  
rem Remove trailing backslash  
set "DOTNET_PATH=%DOTNET_PATH:~0,-1%"  
call :GetTimestamp  
echo [!formatted_datetime!] DOTNET SDK CLI directory: %DOTNET_PATH% >>%logs_dir%  
rem Iterate through all paths in the environment variable Path  
for %%P in ("%PATH:;=" "%") do (  
    set "p=%%~P"  
    rem Remove trailing backslash (if any)  
    if "!p:~-1!"=="\" set "p=!p:~0,-1!"  
    if /i "!p!"=="!DOTNET_PATH!" (  
        goto DOTNET_PATH_Found  
    )  
)  
:DOTNET_PATH_NOT_Found  
call :GetTimestamp  
echo [!formatted_datetime!] ❌ Path "!DOTNET_PATH!" not found in environment variable Path, please add this path to the environment variable Path and try again. >> "%logs_dir%"  
rem DOTNET SDK CLI directory not found, exit directly  
pause  
exit /b  
:DOTNET_PATH_Found  
call :GetTimestamp  
echo [!formatted_datetime!] ✅ DOTNET SDK CLI path "%DOTNET_PATH%" already exists in environment variable Path. >> "%logs_dir%"  
rem DOTNET SDK CLI directory found, continue with subsequent logic  

rem Start gateway build  
rem Copy database configuration directory  
if not exist "%release_dir%\data" (  
    xcopy /E /I "subrepos\GrpcServer\src\config" "%release_dir%"    
) else (  
    echo Database configuration directory^(%release_dir%\data^) already exists, skipping... >>%logs_dir%  
)  
rem Restore gateway solution  
call :GetTimestamp  
echo [!formatted_datetime!] Starting gateway solution build... >>%logs_dir%  
dotnet restore subrepos\GrpcServer\src\IoTCenterHost.sln  
rem Build gateway solution  
dotnet build subrepos\GrpcServer\src\IoTCenterHost.sln --configuration Debug -o "%gateway_dir%" >> "%logs_dir%" 2>&1  
if %ERRORLEVEL% neq 0 (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Error: Gateway build failed, error code %ERRORLEVEL% >> "%logs_dir%"  
    echo Build failed, please check log file: "%logs_dir%"  
    pause  
    exit /b 1  
)  
call :GetTimestamp  
echo [!formatted_datetime!] Gateway solution build completed... >>%logs_dir%  
rem End gateway build  

rem Start Web API build  
rem Restore solution  
call :GetTimestamp  
echo [!formatted_datetime!] Starting WebApi solution build... >>%logs_dir%  
dotnet restore subrepos\WebPlugins\src\back-end\IoTCenterWebApi.sln  
rem Build solution  
dotnet build subrepos\WebPlugins\src\back-end\IoTCenterWebApi\IoTCenterWebApi.csproj --configuration Debug -o "%webapi_dir%" >> "%logs_dir%" 2>&1  
if %ERRORLEVEL% neq 0 (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Error: WebApi build failed, error code %ERRORLEVEL% >> "%logs_dir%"  
    echo Build failed, please check log file: "%logs_dir%"  
    pause  
    exit /b 1  
)  
rem Build plugin output  
dotnet build subrepos\WebPlugins\src\back-end\IoTCenterWebApi.sln  
rem Copy plugin directory to release directory  
move "subrepos\WebPlugins\src\back-end\plugins" "%webapi_dir%"  
rem Copy Windows services directory to release directory  
move "subrepos\WebPlugins\src\back-end\services" "%release_dir%"  
rem Copy shells script directory to release directory  
move ".\shells" "%release_dir%"  
call :GetTimestamp  
echo [!formatted_datetime!] WebApi solution build completed... >>%logs_dir%  
rem End Web API build  

rem Start gateway  
call :GetTimestamp  
echo [!formatted_datetime!] Starting gateway, please wait... >>%logs_dir%  
cd "%gateway_dir%"  
start cmd /k dotnet GWHost1.dll  
timeout /t 2 >nul  
cd /d "%~dp0%"  
call :GetTimestamp  
echo [!formatted_datetime!] Starting WebApi, please wait... >>%logs_dir%  
rem Start Web API  
cd "%webapi_dir%"  
start cmd /k dotnet IoTCenterWebApi.dll  
rem Start Web  
cd /d "%~dp0%"  
call :GetTimestamp  
echo [!formatted_datetime!] Opening IoTCenter access link^(https://localhost:44380^), please wait... >>%logs_dir%  
rem Get default browser protocol association (HTTP)  
for /f "tokens=2*" %%a in ('reg query "HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice" /v ProgId') do set BROWSER=%%b  
rem Determine common browser types and invoke  
if "%BROWSER%"=="ChromeHTML" (  
    start "" "C:\Program Files\Google\Chrome\Application\chrome.exe" --new-window "https://localhost:44380"  
) else if "%BROWSER%"=="MSEdgeHTM" (  
    start "" "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" --new-window "https://localhost:44380"  
) else if "%BROWSER%"=="FirefoxURL" (  
    start "" "C:\Program Files\Mozilla Firefox\firefox.exe" -new-window "https://localhost:44380"  
) else if "%BROWSER%"=="AppXq0fevzme2p429mpbidy6na6cqpiv3i5" (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Microsoft Edge modern version (UWP/Win10+) is not supported. >>%logs_dir%  
) else (  
    call :GetTimestamp  
    echo [!formatted_datetime!] Unrecognized browser type: %BROWSER% >>%logs_dir%  
    echo [!formatted_datetime!] Attempting to open using system default method... >>%logs_dir%  
    start "" "https://localhost:44380"  
)  

rem ========== Function Definitions ==========  
:GetTimestamp  
rem Function: Get current timestamp and store in formatted_datetime variable  
rem Format: yyyy-MM-dd HH:mm:ss  
for /f "delims=" %%a in ('powershell -command "Get-Date -Format 'yyyy-MM-dd HH:mm:ss'"') do (  
    set "formatted_datetime=%%a"  
)  
exit /b