
echo "开始反注册服务"
pwd
echo "当前路径为"$(pwd)
echo "--------------------"
echo "正在反注册IoTCenter 服务"
systemctl kill IoTCenter 
systemctl stop IoTCenter
systemctl disable IoTCenter 
rm -r /etc/systemd/system/IoTCenter.service
echo "IoTCenter服务反注册已完成"
