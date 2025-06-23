@echo off
setlocal enabledelayedexpansion

title Register TOMs Windows Service
	
cd /d "%~dp0%"

rem 设置注册日志目录
set "logs_dir=%cd%\regist_logs.txt"

:: 检测管理员权限
fltmc >nul 2>&1 || (
	call :GetTimestamp
    echo [!formatted_datetime!] Please right-click on this batch file and select "Run as administrator." >>%logs_dir%
    pause
    exit /b
)

if exist "%logs_dir%" (
	call :GetTimestamp
    echo [!formatted_datetime!] Attempting to delete registry log file >>%logs_dir%
	del /q "%logs_dir%"
	if %errorlevel% equ 0 (
		call :GetTimestamp
		echo [!formatted_datetime!] Successfully deleted registry log files >>%logs_dir%
    ) else (
		call :GetTimestamp
		echo [!formatted_datetime!] Failed to delete registry log files^（files might be locked or you lack proper access rights^） >>%logs_dir%
    )
)


rem 检查是否已安装.NET SDK
call :GetTimestamp
echo [!formatted_datetime!] Checking if .NET SDK is already installed... >>%logs_dir%
for /f "tokens=*" %%a in ('dotnet --list-sdks ^| sort /r') do (
    for /f "tokens=2 delims=[]" %%b in ("%%a") do (
        set "SDK_PATH=%%b"
        goto :done
    )
)
:done
if not defined SDK_PATH (
	call :GetTimestamp
	echo [!formatted_datetime!] It has been detected that the .NET SDK is either not installed or its installation path is not included in the global environment variables. Please verify... >>%logs_dir%
    pause
    exit /b 1
)
if "%SDK_PATH%" == "" (
	call :GetTimestamp
	echo [!formatted_datetime!] The SDK installation path is not configured in the global environment variables... >>%logs_dir%
    pause
    exit /b 1
)
if exist "%SDK_PATH%\" (
	 call :GetTimestamp
     echo [!formatted_datetime!] .NET SDK found at location: "%SDK_PATH%"。>>%logs_dir%
) else (
	 call :GetTimestamp
	echo [!formatted_datetime!] The installed .NET SDK path is not properly registered in the system's global environment variables... >>%logs_dir%
    pause
    exit /b 1
)
rem 检查是否已安装.NET SDK 9.0版本
for /f "tokens=*" %%a in ('dotnet --version') do set SDK_VERSION=%%a
call :GetTimestamp
echo [!formatted_datetime!] Current .NET SDK version: %SDK_VERSION% >>%logs_dir%
echo %SDK_VERSION% | findstr "^9" >nul
if %errorlevel% neq 0 (
    call :GetTimestamp
	echo [!formatted_datetime!] Require installation of .NET SDK version 9.0 >>%logs_dir%
	pause
    exit /b 1
)
rem 检查是dotnet命令是否已加入到环境变量path中
for %%A in ("%SDK_PATH%") do set "DOTNET_PATH=%%~dpA"
rem 去除末尾的反斜杠
set "DOTNET_PATH=%DOTNET_PATH:~0,-1%"
call :GetTimestamp
echo [!formatted_datetime!] DOTNET SDK CLI Dicrectory：%DOTNET_PATH% >>%logs_dir%
rem 遍历环境变量Path中的所有路径
for %%P in ("%PATH:;=" "%") do (
	set "p=%%~P"
    rem 去除末尾的反斜杠（如果有的话）
    if "!p:~-1!"=="\" set "p=!p:~0,-1!"
    if /i "!p!"=="!DOTNET_PATH!" (
        goto DOTNET_PATH_Found
    )
)
:DOTNET_PATH_NOT_Found
call :GetTimestamp
echo [!formatted_datetime!] Path "!DOTNET_PATH!" Not found in the environment variable Path. Please add the path to the environment variable Path and try again。>> "%logs_dir%"
rem DOTNET SDK CLI目录未找到直接退出
pause
exit /b
:DOTNET_PATH_Found
call :GetTimestamp
echo [!formatted_datetime!] DOTNET SDK CLI Path "%DOTNET_PATH%" already exists in the environment variable Path。>> "%logs_dir%"

rem sc query IoTCenterDaemon |findstr /i "STATE">nul
rem if not errorlevel 1 (
rem 	goto startDaemon
rem ) else (
rem 	goto createDaemon
rem ) 

rem :startDaemon
rem sc start "IoTCenterDaemon"

rem :createDaemon

rem set "DAEMON_SERVICE_PATH=%~dp0services\daemon"

cd ..

set "DAEMON_SERVICE_PATH=%cd%\services\daemon"
set "BIN_SERVICE_PATH=%cd%\bin"
set "WEB_SERVICE_PATH=%cd%\IoTCenterWeb"

cd /d "%dp0%"

rem call :GetTimestamp
rem echo [!formatted_datetime!] 正在安装IoTCenterDaemon服务 >> "%logs_dir%"

rem sc create IoTCenterDaemon binPath= "\"%DOTNET_PATH%\dotnet.exe" \"%DAEMON_SERVICE_PATH%\IoTCenterWeb.Daemon.dll"" DisplayName= "IoTCenterDaemon" start= auto  
rem sc start "IoTCenterDaemon"

rem call :GetTimestamp
rem echo [!formatted_datetime!] IoTCenterDaemon服务安装完成 >> "%logs_dir%"

sc query IoTCenter |findstr /i "STATE">nul
if not errorlevel 1 (goto exist) else goto notexist
:exist
sc start "IoTCenter"
:notexist
call :GetTimestamp
echo [!formatted_datetime!] Installing IoTCenter service. >> "%logs_dir%"
sc create IoTCenter binPath= "%BIN_SERVICE_PATH%\GWHost1.exe" DisplayName= "IoTCenter Service" start= auto  
sc start "IoTCenter"
call :GetTimestamp
echo [!formatted_datetime!] The IoTCenter service installation has been completed. >> "%logs_dir%"

goto Web
:Web
sc query IoTCenterWeb |findstr /i "STATE">nul
if not errorlevel 1 (goto exist) else goto notexist
:exist
sc start "IoTCenterWeb"
pause
goto end
:notexist
call :GetTimestamp
echo [!formatted_datetime!] Installing IoTCenterWeb service >> "%logs_dir%"
sc create IoTCenterWeb binPath= "%WEB_SERVICE_PATH%\publish\IoTCenterWebApi.exe" DisplayName= "IoTCenterWeb Service" start= auto
sc start "IoTCenterWeb"
call :GetTimestamp
echo [!formatted_datetime!] The IoTCenterWeb service installation has been completed >> "%logs_dir%"

rem 启动Web
call :GetTimestamp
echo [!formatted_datetime!] Opening the IoTCenter access link^(https://localhost:44380^)，Please Wait... >>%logs_dir%
rem 获取默认浏览器的协议关联（HTTP）
for /f "tokens=2*" %%a in ('reg query "HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice" /v ProgId') do set BROWSER=%%b
rem 判断常见浏览器类型并调用
if "%BROWSER%"=="ChromeHTML" (
    start "" "C:\Program Files\Google\Chrome\Application\chrome.exe" --new-window "https://localhost:44380"
) else if "%BROWSER%"=="MSEdgeHTM" (
    start "" "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" --new-window "https://localhost:44380"
) else if "%BROWSER%"=="FirefoxURL" (
    start "" "C:\Program Files\Mozilla Firefox\firefox.exe" -new-window "https://localhost:44380"
) else if "%BROWSER%"=="AppXq0fevzme2p429mpbidy6na6cqpiv3i5" (
    call :GetTimestamp
	echo [!formatted_datetime! Modern versions of Microsoft Edge browser ^(UWP/Win10+ based^) are not supported。>>%logs_dir%
) else (
	call :GetTimestamp
    echo [!formatted_datetime!] Unrecognized browser type：%BROWSER% >>%logs_dir%
    echo [!formatted_datetime!] Attempting to open using the system default method... >>%logs_dir%
    start "" "https://localhost:44380"
)

rem ========== 函数定义 ==========
:GetTimestamp
rem 功能：获取当前时间戳并存入formatted_datetime变量
rem 格式：yyyy-MM-dd HH:mm:ss
for /f "delims=" %%a in ('powershell -command "Get-Date -Format 'yyyy-MM-dd HH:mm:ss'"') do (
    set "formatted_datetime=%%a"
)
exit /b

pause