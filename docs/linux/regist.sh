echo "正在注册服务"
pwd
echo "当前路径为"$(pwd)
echo "--------------------"
echo "正在注册服务IoTCenter"
cp IoTCenter.service /etc/systemd/system/
systemctl daemon-reload
systemctl enable IoTCenter
systemctl start IoTCenter
echo "IoTCenter服务注册已完成"
 
echo "正在初始化配置"
FILE=/opt/ganwei/IoTCenter/data/AlarmCenter/AlarmCenterProperties-linux.xml
if [ -f "$FILE" ]; then
	mv /opt/ganwei/IoTCenter/data/AlarmCenter/AlarmCenterProperties-linux.xml /opt/ganwei/IoTCenter/data/AlarmCenter/AlarmCenterProperties.xml
fi
echo "初始化配置已完成"

echo "--------------------"
echo "服务注册已完成"
echo "--------------------"