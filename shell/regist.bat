@echo off

rem 切换到当前批处理文件所在目录
cd /d "%~dp0%"

chcp 65001 >nul

rem 设置构建日志目录
set "logs_dir=regist_logs.txt"

rem 检查是否已安装.NET SDK
call :GetTimestamp
echo 正在检测是否已安装.NET SDK... >>%logs_dir%
for /f "tokens=*" %%a in ('dotnet --list-sdks ^| sort /r') do (
    for /f "tokens=2 delims=[]" %%b in ("%%a") do (
        set "SDK_PATH=%%b"
        goto :done
    )
)
:done
if not defined SDK_PATH (
	echo 已检测到.NET SDK未安装或安装路径未在全局环境变量中，请确认... >>%logs_dir%
    pause
    exit /b 1
)
if "%SDK_PATH%" == "" (
	echo 已检测到.NET SDK未安装或安装路径未在全局环境变量中，请确认... >>%logs_dir%
    pause
    exit /b 1
)
if exist "%SDK_PATH%\" (
     echo .NET SDK路径"%SDK_PATH%"已安装。>>%logs_dir%
) else (
	echo 已检测到.NET SDK未安装或安装路径未在全局环境变量中，请确认... >>%logs_dir%
    pause
    exit /b 1
)
rem 检查是否已安装.NET SDK 9.0版本
for /f "tokens=*" %%a in ('dotnet --version') do set SDK_VERSION=%%a
call :GetTimestamp
echo [!formatted_datetime!] 当前.NET SDK 版本: %SDK_VERSION% >>%logs_dir%
echo %SDK_VERSION% | findstr "^9" >nul
if %errorlevel% neq 0 (
    echo 要求安装.NET SDK为9.0版本
	echo 要求安装.NET SDK为9.0版本 >>%logs_dir%
	pause
    exit /b 1
)
rem 检查是dotnet命令是否已加入到环境变量path中
for %%A in ("%SDK_PATH%") do set "DOTNET_PATH=%%~dpA"
rem 去除末尾的反斜杠
set "DOTNET_PATH=%DOTNET_PATH:~0,-1%"
echo DOTNET SDK CLI目录：%DOTNET_PATH% >>%logs_dir%
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
echo ❌ 路径 "!DOTNET_PATH!" 未在环境变量Path中找到，请将该路径添加至环境变量Path中再次重试。>> "%logs_dir%"
rem DOTNET SDK CLI目录未找到直接退出
pause
exit /b
:DOTNET_PATH_Found
echo  ✅ DOTNET SDK CLI路径 "%DOTNET_PATH%" 已存在于环境变量Path中。>> "%logs_dir%"

echo Start Install IoTCenter Service

sc query IoTCenterDaemon |findstr /i "STATE">nul
if not errorlevel 1 (goto startDaemon) else goto createDaemon

:startDaemon
sc start "IoTCenterDaemon"

:createDaemon

cd ..\

set DAEMON_SERVICE_PATH=%cd%\services\daemon
set BIN_SERVICE_PATH=%cd%\bin

sc create IoTCenterDaemon binPath= "\"%DOTNET_PATH%\dotnet.exe\" \"%DAEMON_SERVICE_PATH%\IoTCenterWeb.Daemon.dll\"" DisplayName= "IoTCenterDaemon" start= auto  
sc start "IoTCenterDaemon"
echo IoTCenterDaemon Installed Successfully

@echo off
sc query IoTCenter |findstr /i "STATE">nul
if not errorlevel 1 (goto exist) else goto notexist 

:exist
sc start "IoTCenter"


:notexist
sc create IoTCenter binPath= "\"%DOTNET_PATH%\dotnet.exe\" \"%BIN_SERVICE_PATH%\GWHost1.dll\"" DisplayName= "IoTCenter Service" start= auto  
sc start "IoTCenter"
echo IoTCenter Installed Successfully

pause