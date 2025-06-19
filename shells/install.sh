#!/bin/bash

# 先执行设置脚本
source ./IoTCenterWeb/shell/environment.sh

# 打印安装目录
echo "install path: $GLOBAL_INSTALL_PATH"

echo "启动IoTCenter安装程序"
echo "-----------------"

# 检测系统架构
get_arch=$(arch)
echo "系统架构版本：$get_arch"
osName="Linux_x86_64"
if [ "$get_arch" = "aarch64" ]; then
  osName="Arm64"
  echo "-------arm平台---------"
fi

# 检查目标目录是否已存在
if [ -d "$GLOBAL_INSTALL_PATH" ]; then
  echo "$GLOBAL_INSTALL_PATH 目标路径文件夹已存在，安装结束。"
  exit 0
fi

# 创建安装目录
mkdir -p "$GLOBAL_INSTALL_PATH"

echo "正在解压安装包到目标目录..."

tar -zxvf "$osName.tar.gz" -C "$GLOBAL_INSTALL_PATH/"

echo "正在注册服务"
cd "$GLOBAL_INSTALL_PATH/services/" || { echo "无法进入服务目录！"; exit 1; }
pwd
chmod u+x regist.sh
./regist.sh

echo "-----------------"
echo "安装程序执行完毕"
exit 0