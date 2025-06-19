@echo off

cd /d "%~dp0%"

chcp 65001 >nul

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