@echo off

cd /d "%~dp0%"

@echo off

rem Unified script for both initial setup and updates
rem Check if repository exists
if not exist "TOMs\.git" (
    echo Initial clone required...
    git clone https://github.com/ganweisoft/TOMs.git
    cd TOMs
) else (
    echo Repository exists, pulling updates...
    cd TOMs
    git pull
)

rem Common operations for both cases
git submodule init
git submodule update --remote
call build.bat