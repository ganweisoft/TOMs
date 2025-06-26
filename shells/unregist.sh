echo "Starting service unregistration"
pwd
echo "Current path is "$(pwd)
echo "--------------------"
echo "Unregistering IoTCenter service"
systemctl kill IoTCenter
systemctl stop IoTCenter
systemctl disable IoTCenter
rm -r /etc/systemd/system/IoTCenter.service
echo "IoTCenter service unregistration completed"