#!/bin/bash

# First execute the setup script
source environment.sh

# Print installation directory
echo "install path: $GLOBAL_INSTALL_PATH"

echo "Starting IoTCenter installation"
echo "-----------------"

# Detect system architecture
get_arch=$(arch)
echo "System architecture version: $get_arch"
osName="Linux_x86_64"
if [ "$get_arch" = "aarch64" ]; then
  osName="Arm64"
  echo "-------ARM platform---------"
fi

# Check if target directory already exists
if [ -d "$GLOBAL_INSTALL_PATH" ]; then
  echo "$GLOBAL_INSTALL_PATH target directory already exists, installation aborted."
  exit 0
fi

# Create installation directory
mkdir -p "$GLOBAL_INSTALL_PATH"

echo "Extracting package to target directory..."

tar -zxvf "$osName.tar.gz" -C "$GLOBAL_INSTALL_PATH/"

echo "Registering service"
cd "$GLOBAL_INSTALL_PATH/services/" || { echo "Failed to enter service directory!"; exit 1; }
pwd
chmod u+x regist.sh
./regist.sh

echo "-----------------"
echo "Installation completed"
exit 0