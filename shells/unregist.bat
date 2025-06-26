@echo off
setlocal enabledelayedexpansion

title Unregister TOMs Windows Service
	
cd /d "%~dp0%"

rem 设置卸载日志目录
set "logs_dir=%cd%\unregist_logs.txt"

:: 检测管理员权限
fltmc >nul 2>&1 || (
	call :GetTimestamp
    echo [!formatted_datetime!] Please right-click on this batch file and select "Run as administrator."" >>%logs_dir%
    pause
    exit /b
)

if exist "%logs_dir%" (
	del /q "%logs_dir%"
	if %errorlevel% equ 0 (
		call :GetTimestamp
		echo [!formatted_datetime!] Successfully deleted registry log files: "%logs_dir%" >>%logs_dir%
    ) else (
		call :GetTimestamp
		echo [!formatted_datetime!] Failed to delete registry log files^（files might be locked or you lack proper access rights^） >>%logs_dir%
    )
)

rem sc stop IoTCenterDaemon 
rem sc delete IoTCenterDaemon 
rem echo IoTCenterDaemon UnInstalled Successfully

sc query IoTCenter |findstr /i "STATE">nul
if not errorlevel 1 (goto exist) else goto notexist
 
:exist
sc stop IoTCenter 
sc delete IoTCenter

call :GetTimestamp
echo [!formatted_datetime!] IoTCenter UnInstalled Successfully  >>%logs_dir%

:notexist

sc query IoTCenterWeb |findstr /i "STATE">nul
if not errorlevel 1 (goto exist) else goto notexist
 
:exist
sc stop IoTCenterWeb 
sc delete IoTCenterWeb

call :GetTimestamp
echo [!formatted_datetime!] IoTCenterWeb UnInstalled Successfully  >>%logs_dir%

:notexist

rem ========== 函数定义 ==========
:GetTimestamp
rem 功能：获取当前时间戳并存入formatted_datetime变量
rem 格式：yyyy-MM-dd HH:mm:ss
for /f "delims=" %%a in ('powershell -command "Get-Date -Format 'yyyy-MM-dd HH:mm:ss'"') do (
    set "formatted_datetime=%%a"
)
exit /b

pause
