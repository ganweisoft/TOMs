#!/bin/bash

# 先执行设置脚本
source environment.sh

# 打印安装目录
echo "install path: $GLOBAL_INSTALL_PATH"

while true
do
    host_dir=`echo ~`                                       # 当前用户根目录
    proc_name="IoTCenterWebApi.dll"                             # 进程名
    file_name="$GLOBAL_INSTALL_PATH/IoTCenterWeb/shell/web.log"                         # 日志文件
    pid=0

    proc_num()                                              # 计算进程数
    {
        num=`ps -ef | grep $proc_name | grep -v grep | wc -l`
        echo "当前进程数:"$num
        return $num
    }

    proc_id()                                               # 进程号
    {
        pid=`ps -ef | grep $proc_name | grep -v grep | awk '{print $2}'`
        echo "当前进程号："$pid
    }


    proc_num
    number=$?
    proc_id
    echo "number:"$number
    if [ $number -eq 0 ]                                    # 判断进程是否存在
    then
        cd $GLOBAL_INSTALL_PATH/IoTCenterWeb/publish
        pwd
        nohup dotnet IoTCenterWebApi.dll > /dev/null 2>&1 &
                                                            # 重启进程的命令，请相应修改
        proc_id                                         # 获取新进程号
        echo "IoTCenterWebApi当前进程Id："${pid}, `date` >> $file_name      # 将新进程号和重启时间记录
    fi
    sleep 10
done
