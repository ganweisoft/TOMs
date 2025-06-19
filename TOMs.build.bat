@echo off

cd /d "%~dp0%"

rem =============If this is the first time cloning the repository for building, execute the following code============= 
git clone https://github.com/ganweisoft/TOMs.git
cd TOMs
git submodule init
git submodule update
build.bat

rem ============= Otherwise, execute the following code=============
rem git pull
rem git submodule update --remote
rem build.bat