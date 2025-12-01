@echo off
setlocal enabledelayedexpansion

rem 切换到当前批处理文件所在目录
cd /d "%~dp0%"

chcp 65001 >nul

rem 设置构建日志目录
set "logs_dir=build_logs.txt"
del /q /f %logs_dir%
call :GetTimestamp
echo [!formatted_datetime!] 删除已生成的构建日志文件 >>%logs_dir%
rem 设置实际发布目录
set "release_dir=.\Release"
call :GetTimestamp
echo [!formatted_datetime!] 设置整体发布目录路径: %release_dir% >>%logs_dir%
rem 设置网关发布目录
set "gateway_dir=%release_dir%\bin"
call :GetTimestamp
echo [!formatted_datetime!] 设置网关发布目录路径: %gateway_dir% >>%logs_dir%
rem 设置WebAPi发布目录
set "webapi_dir=%release_dir%\IoTCenterWeb\publish"
call :GetTimestamp
echo [!formatted_datetime!] 设置webapi发布目录路径: %webapi_dir% >>%logs_dir%
rem 设置前端发布目录
set "wwwroot_dir=%webapi_dir%\wwwroot"
rem 设置 nvm 安装程序路径
set "nvm_dir=repos\WebPlugins\src\front-end\nvm-setup.exe"
call :GetTimestamp
echo [!formatted_datetime!] 设置nvm离线安装程序路径: %nvm_dir% >>%logs_dir%
rem 创建发布目录并清空旧目录
if exist "%release_dir%" (
	call :GetTimestamp
    echo [!formatted_datetime!] 发布目录^(%release_dir%^)已存在，删除中... >>%logs_dir%
    rd /s /q "%release_dir%"
	if errorlevel 1 (
		call :GetTimestamp
		echo [!formatted_datetime!] 删除发布目录失败！可能有文件被占用或权限不足。 >>%logs_dir%
	)
	call :GetTimestamp
    echo [!formatted_datetime!] 发布目录^(%release_dir%^)已删除... >>%logs_dir%
	mkdir "%release_dir%"
	call :GetTimestamp
    echo [!formatted_datetime!] 发布目录^(%release_dir%^)已重新创建完成... >>%logs_dir%
) else (
	call :GetTimestamp
    echo [!formatted_datetime!] 发布目录^(%release_dir%^)不存在，创建中... >>%logs_dir%
    mkdir "%release_dir%"
    echo [!formatted_datetime!] 发布目录^(%release_dir%^)已创建完成... >>%logs_dir%
)
rem 开始前端构建
rem ==============================
rem 检查并强制安装nvm开始
rem ==============================
:CheckNVMInstall
if defined NVM_HOME (
	call :GetTimestamp
	echo [!formatted_datetime!] 检测nvm已安装，安装路径为: %NVM_HOME% >>%logs_dir%
	goto ContinueScript
)
if exist "%nvm_dir%" (
	if defined NVM_HOME (
		call :GetTimestamp
		echo [!formatted_datetime!] 检测nvm已安装，安装路径为: %NVM_HOME% >>%logs_dir%
	) else (
		call :GetTimestamp
		echo [!formatted_datetime!] 正在安装nvm，请稍后... >>%logs_dir%
		call "%nvm_dir%"
		goto ContinueScript
	)
) else (
	echo [!formatted_datetime!] 您可能尚未安装nvm-setup程序，请在目录（repos\WebPlugins\src\front-end\nvm-setup.exe）手动安装后再次重试。>>%logs_dir%
	timeout /t 10 >nul
	goto CheckNVMInstall
)
:ContinueScript
rem 尝试从注册表读取用户环境变量中的NVM_HOME
call :GetTimestamp
echo [!formatted_datetime!] 正在刷新%NVM_HOME%注册表，重新获取nvm环境变量... >>%logs_dir%
for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v NVM_HOME 2^>nul') do set "NVM_HOME=%%b"
if defined NVM_HOME (
   call :GetTimestamp
   echo [!formatted_datetime!] 已完成nvm安装（%NVM_HOME%），继续执行后续操作... >>%logs_dir%
) else (
    goto CheckNVMInstall
)
rem ==============================
rem 检查并强制安装nvm结束
rem ==============================
set "NODE_VERSION1=14.21.3"
set "NODE_VERSION2=23.1.0"
call :GetTimestamp
echo [!formatted_datetime!] 正在检查 Node.js %NVM_HOME%\v%NODE_VERSION1% 是否已安装... >>%logs_dir%
if exist "%NVM_HOME%\v%NODE_VERSION1%" (
	call :GetTimestamp
	echo [!formatted_datetime!] Node.js v%NODE_VERSION1% 已安装。>>%logs_dir%
) else (
	call :GetTimestamp
	echo [!formatted_datetime!] Node.js v%NODE_VERSION1% 未安装，正在安装... >>%logs_dir%
	"%NVM_HOME%\nvm" install %NODE_VERSION1% --npm-site https://npm.taobao.org/mirrors/node
	call :GetTimestamp
	echo [!formatted_datetime!] Node.js v%NODE_VERSION1% 已完成安装... >>%logs_dir%
)
call :GetTimestamp
echo [!formatted_datetime!] 正在检查 Node.js %NVM_HOME%\v%NODE_VERSION2% 是否已安装... >>%logs_dir%
if exist "%NVM_HOME%\v%NODE_VERSION2%" (
	call :GetTimestamp
	echo [!formatted_datetime!] Node.js v%NODE_VERSION2% 已安装。>>%logs_dir%
) else (
	call :GetTimestamp
	echo [!formatted_datetime!] Node.js v%NODE_VERSION2% 未安装，正在安装... >>%logs_dir%
    "%NVM_HOME%\nvm" install %NODE_VERSION2% --npm-site https://npm.taobao.org/mirrors/node
	echo [!formatted_datetime!] Node.js v%NODE_VERSION2% 已完成安装... >>%logs_dir%
    "%NVM_HOME%\nvm" use %NODE_VERSION2%
	"%NVM_HOME%\nvm" install pnpm -g
	call :GetTimestamp
	echo [!formatted_datetime!] Node.js v%NODE_VERSION2% pnpm已完成安装... >>%logs_dir%
)
call :GetTimestamp
echo [!formatted_datetime!] 开始构建前端解决方案... >>%logs_dir%
rem 尝试从注册表读取用户环境变量中的NVM_SYMLINK
for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v NVM_SYMLINK 2^>nul') do set "NVM_SYMLINK=%%b"
call :GetTimestamp
echo [!formatted_datetime!] 正在刷新【%NODE_VERSION1%】^(%NVM_SYMLINK%^)注册表，重新获取nodejs环境变量... >>%logs_dir%
set "NODE_PATH=%NVM_SYMLINK%\node.exe"
if defined NVM_SYMLINK (
	"%NVM_HOME%\nvm" use %NODE_VERSION1%
	PowerShell -Command "Test-Path '%NODE_PATH%'" | findstr /i "True" >nul
	if %errorlevel% == 0 (
		"%NVM_SYMLINK%\node" repos\WebPlugins\src\front-end\build.js
		call :GetTimestamp
		echo [!formatted_datetime!] 前端插件已构建完成... >>%logs_dir%
	) else (
		call :GetTimestamp
		echo [!formatted_datetime!] Node.js v%NODE_VERSION1% 可能因网络原因安装失败... >>%logs_dir%
		echo [!formatted_datetime!] 请执行手动卸载 %NVM_HOME%\unins000.exe 再次重试或者确认其他命令窗口已打开导致占用，关闭后再次重试... >>%logs_dir%
		pause
		exit /b
	)
	"%NVM_HOME%\nvm" use %NODE_VERSION2%
	rem 尝试从注册表读取用户环境变量中的NVM_SYMLINK
	for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v NVM_SYMLINK 2^>nul') do set "NVM_SYMLINK=%%b"
	call :GetTimestamp
	echo [!formatted_datetime!] 正在刷新【%NODE_VERSION2%】^(%NVM_SYMLINK%^)注册表，重新获取nodejs环境变量... >>%logs_dir%
	PowerShell -Command "Test-Path '%NODE_PATH%'" | findstr /i "True" >nul
	if %errorlevel% == 0 (
		"%NVM_SYMLINK%\node" repos\WebPlugins\src\front-end\build.js
		call :GetTimestamp
		echo [!formatted_datetime!] 脚手架已构建完成... >>%logs_dir%
	) else (
		call :GetTimestamp
		echo [!formatted_datetime!] Node.js v%NODE_VERSION2% 可能因网络原因安装失败... >>%logs_dir%
		echo [!formatted_datetime!] 请执行手动卸载 %NVM_HOME%\unins000.exe 再次重试或者确认其他命令窗口是否已打开导致占用，关闭后再次重试... >>%logs_dir%
		pause
		exit /b
	)
) else (
	call :GetTimestamp
	echo [!formatted_datetime!] Node.js未安装，构建前端解决方案失败... >>%logs_dir%
	echo [!formatted_datetime!] 请执行手动卸载 %NVM_HOME%\unins000.exe 再次重试... >>%logs_dir%
	pause
	exit /b
)
call :GetTimestamp
echo [!formatted_datetime!] 完成构建前端解决方案... >>%logs_dir%
rem 结束前端构建

rem 检查是否已安装.NET SDK
call :GetTimestamp
echo [!formatted_datetime!] 正在检测是否已安装.NET SDK... >>%logs_dir%
for /f "tokens=*" %%a in ('dotnet --list-sdks ^| sort /r') do (
    for /f "tokens=2 delims=[]" %%b in ("%%a") do (
        set "SDK_PATH=%%b"
        goto :done
    )
)
:done
if not defined SDK_PATH (
	call :GetTimestamp
	echo [!formatted_datetime!] 已检测到.NET SDK未安装或安装路径未在全局环境变量中，请确认... >>%logs_dir%
    echo 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0
	echo [!formatted_datetime!] 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%
    pause
    exit /b 1
)
if "%SDK_PATH%" == "" (
    call :GetTimestamp
	echo [!formatted_datetime!] 已检测到.NET SDK未安装或安装路径未在全局环境变量中，请确认... >>%logs_dir%
    echo 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0
	echo [!formatted_datetime!] 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%
    pause
    exit /b 1
)
if exist "%SDK_PATH%\" (
	 call :GetTimestamp
     echo [!formatted_datetime!] .NET SDK路径"%SDK_PATH%"已安装。>>%logs_dir%
) else (
	call :GetTimestamp
	echo [!formatted_datetime!] 已检测到.NET SDK未安装或安装路径未在全局环境变量中，请确认... >>%logs_dir%
    echo 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0
	call :GetTimestamp
	echo [!formatted_datetime!] 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%
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
	call :GetTimestamp
	echo [!formatted_datetime!] 要求安装.NET SDK为9.0版本 >>%logs_dir%
	echo 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0
	call :GetTimestamp
	echo [!formatted_datetime!] 下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/9.0 >>%logs_dir%
	pause
    exit /b 1
)
rem 检查dotnet命令是否已加入到环境变量path中
for %%A in ("%SDK_PATH%") do set "DOTNET_PATH=%%~dpA"
rem 去除末尾的反斜杠
set "DOTNET_PATH=%DOTNET_PATH:~0,-1%"
call :GetTimestamp
echo [!formatted_datetime!] DOTNET SDK CLI目录：%DOTNET_PATH% >>%logs_dir%
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
echo [!formatted_datetime!] ❌ 路径 "!DOTNET_PATH!" 未在环境变量Path中找到，请将该路径添加至环境变量Path中再次重试。>> "%logs_dir%"
rem DOTNET SDK CLI目录未找到直接退出
pause
exit /b
:DOTNET_PATH_Found
call :GetTimestamp
echo [!formatted_datetime!] ✅ DOTNET SDK CLI路径 "%DOTNET_PATH%" 已存在于环境变量Path中。>> "%logs_dir%"
rem DOTNET SDK CLI目录已找到继续执行后续逻辑
rem 开始网关构建
rem 复制数据库配置目录
if not exist "%release_dir%\data" (
    xcopy "repos\GrpcServer\src\config" "%release_dir%" /E /I
) else (
    echo 数据库配置目录^(%release_dir%\data^)已存在，跳过处理... >>%logs_dir%
)
rem 还原网关解决方案
call :GetTimestamp
echo [!formatted_datetime!] 开始构建网关解决方案... >>%logs_dir%
dotnet restore repos\GrpcServer\src\IoTCenterHost.sln
rem 构建网关解决方案
dotnet build repos\GrpcServer\src\IoTCenterHost.sln --configuration Debug -o "%gateway_dir%" >> "%logs_dir%" 2>&1
if %ERRORLEVEL% neq 0 (
    call :GetTimestamp
    echo [!formatted_datetime!] 错误：网关构建失败，错误代码 %ERRORLEVEL% >> "%logs_dir%"
    echo 构建失败，请查看日志文件："%logs_dir%"
    pause
    exit /b 1
)
call :GetTimestamp
echo [!formatted_datetime!] 完成构建网关解决方案... >>%logs_dir%
rem 结束网关构建
rem 开始Web APi构建
rem 还原解决方案
call :GetTimestamp
echo [!formatted_datetime!] 开始构建WebApi解决方案... >>%logs_dir%
dotnet restore repos\WebPlugins\src\back-end\IoTCenterWebApi.sln
rem 构建解决方案
dotnet build repos\WebPlugins\src\back-end\IoTCenterWebApi\IoTCenterWebApi.csproj --configuration Debug -o "%webapi_dir%" >> "%logs_dir%" 2>&1
if %ERRORLEVEL% neq 0 (
    call :GetTimestamp
    echo [!formatted_datetime!] 错误：WebApi构建失败，错误代码 %ERRORLEVEL% >> "%logs_dir%"
    echo 构建失败，请查看日志文件："%logs_dir%"
    pause
    exit /b 1
)
rem 构建插件输出
dotnet build repos\WebPlugins\src\back-end\IoTCenterWebApi.sln
rem 复制插件目录至发布目录
move "repos\WebPlugins\src\back-end\plugins" "%webapi_dir%"
rem 复制windows服务目录至发布目录
move "repos\WebPlugins\src\back-end\services" "%release_dir%"
rem 复制shells脚本目录至发布目录
xcopy ".\shells" "%release_dir%\shells" /E /I
call :GetTimestamp
echo [!formatted_datetime!] 完成构建WebApi解决方案... >>%logs_dir%
rem 结束Web APi构建

rem ========== 函数定义 ==========
:GetTimestamp
rem 功能：获取当前时间戳并存入formatted_datetime变量
rem 格式：yyyy-MM-dd HH:mm:ss
for /f "delims=" %%a in ('powershell -command "Get-Date -Format 'yyyy-MM-dd HH:mm:ss'"') do (
    set "formatted_datetime=%%a"
)
exit /b