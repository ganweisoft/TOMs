@echo off
setlocal enabledelayedexpansion

rem 切换到当前批处理文件所在目录
cd /d "%~dp0%"

chcp 65001 >nul

set "logs_dir=run_logs.txt"
del /q /f %logs_dir%
rem 设置网关发布目录
set "gateway_dir=%release_dir%\bin"
rem 设置WebAPi发布目录
set "webapi_dir=%release_dir%\IoTCenterWeb\publish"

rem Unified script for both initial setup and updates
rem Check if repository exists
dir /ad ".git" >nul 2>&1
if %errorlevel% == 0 (
	echo Repository exists, pulling updates...
    git pull
) else (
	echo Initial clone required...
    git clone https://github.com/ganweisoft/TOMs.git
    cd TOMs
)
rem Common operations for both cases
git submodule init
git submodule update --remote
if not exist "build.bat" (
    echo build.bat not exist
	pause
	exit /b
)
call build.bat
call :GetTimestamp
echo [!formatted_datetime!] 正在启动网关，请稍后... >>%logs_dir%
cd "%gateway_dir%"
start cmd /k dotnet GWHost1.dll
timeout /t 2 >nul
cd /d "%~dp0%"
call :GetTimestamp
echo [!formatted_datetime!] 正在启动WebApi，请稍后... >>%logs_dir%
rem 启动Web APi
cd "%webapi_dir%"
start cmd /k dotnet IoTCenterWebApi.dll
rem 启动Web
cd /d "%~dp0%"
call :GetTimestamp
echo [!formatted_datetime!] 正在打开IoTCenter访问链接^(https://localhost:44380^)，请稍后... >>%logs_dir% rem --------------
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
	echo [!formatted_datetime! 不支持 Microsoft Edge 浏览器的现代版本（基于 UWP/Win10+）。>>%logs_dir%
) else (
	call :GetTimestamp
    echo [!formatted_datetime!] 未识别的浏览器类型：%BROWSER% >>%logs_dir%
    echo [!formatted_datetime!] 正在尝试使用系统默认方式打开... >>%logs_dir%
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