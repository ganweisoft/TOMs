# Execute setup script
source environment.sh

# Print installation directory
echo "Install path: $GLOBAL_INSTALL_PATH"

echo "Registering service"
pwd
echo "Current path is "$(pwd)
echo "--------------------"
echo "Registering IoTCenter service"
cp IoTCenter.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable IoTCenter
systemctl start IoTCenter
echo "IoTCenter service registration completed"

echo "Initializing configuration"
FILE=$GLOBAL_INSTALL_PATH/data/AlarmCenter/AlarmCenterProperties-linux.xml
if [ -f "$FILE" ]; then
    mv $GLOBAL_INSTALL_PATH/data/AlarmCenter/AlarmCenterProperties-linux.xml $GLOBAL_INSTALL_PATH/data/AlarmCenter/AlarmCenterProperties.xml
fi
echo "Configuration initialization completed"

echo "--------------------"
echo "Service registration completed"
echo "--------------------"