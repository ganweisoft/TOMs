echo "启动IoTCenter安装程序"

echo "-----------------"
get_arch=`arch`
echo "系统架构版本：$get_arch"
osName="Linux_x86_64"
if [ "$get_arch" = "aarch64" ];then
  osName="Arm64"
  echo -------arm平台---------
fi

path="/opt/ganwei"
if [ ! -x "$path/IoTCenter" ]; then
   mkdir "$path"
else 
   echo "/opt/ganwei/IoTCenter目标路径文件夹已存在，安装结束。"
   exit 0
fi
 
echo "正在解压安装包" 
tar -zxvf "$osName.tar.gz" -C "$path/"
cd $path
mv $osName "IoTCenter" 

echo "正在注册服务"
cd $path/IoTCenter/services/
pwd
chmod u+x regist.sh
sh regist.sh

echo "软连接dotnet"
ln -s /opt/ganwei/IoTCenter/dotnet/dotnet /usr/bin/dotnet

echo "-----------------"
echo "安装程序执行完毕"
