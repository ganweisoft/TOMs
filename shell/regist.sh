
# 执行设置脚本
source ../IoTCenterWeb/shell/environment.sh

# 打印安装目录
echo "install path: $GLOBAL_INSTALL_PATH"

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
FILE=$GLOBAL_INSTALL_PATH/data/AlarmCenter/AlarmCenterProperties-linux.xml
if [ -f "$FILE" ]; then
	mv $GLOBAL_INSTALL_PATH/data/AlarmCenter/AlarmCenterProperties-linux.xml $GLOBAL_INSTALL_PATH/data/AlarmCenter/AlarmCenterProperties.xml
fi
echo "初始化配置已完成"

echo "--------------------"
echo "服务注册已完成"
echo "--------------------"