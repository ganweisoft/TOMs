#!/bin/bash

# First execute the setup script
source ./environment.sh

echo "Starting TOMs installation"
echo "-----------------"

echo "Registering IoTCenter service"
nohup sh restart.sh > /dev/null 2>&1 &
echo "Registred IoTCenter service"

echo "Registering IoTCenterWeb service"
nohup sh restartweb.sh > /dev/null 2>&1 &
echo "Registred IoTCenterWeb service"

echo "-----------------"
echo "Installation completed"
exit 0