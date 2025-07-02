#!/bin/bash

# First execute the setup script
source ./environment.sh

parent_dir=$(dirname "$GLOBAL_INSTALL_PATH")

while true
do
    host_dir=`echo ~`                                       # Current user home directory
    proc_name="GWHost1.dll"                                 # Process name
    file_name="$parent_dir/IoTCenterWeb/shell/grpc.log"  # Log file
    pid=0

    proc_num()                                              # Calculate process count
    {
        num=`ps -ef | grep $proc_name | grep -v grep | wc -l`
        echo "Current process count:"$num
        return $num
    }

    proc_id()                                               # Get process ID
    {
        pid=`ps -ef | grep $proc_name | grep -v grep | awk '{print $2}'`
        echo "Current process ID:"$pid
    }

    proc_num
    number=$?
    proc_id
    echo "number:"$number
    if [ $number -eq 0 ]                                    # Check if process exists
    then
        nohup dotnet $parent_dir/bin/GWHost1.dll > /dev/null 2>&1 &
                                                            # Command to restart process (modify as needed)
        proc_id                                         # Get new process ID
        echo ${pid}, `date` >> $file_name      # Record new process ID and restart time
    fi
    sleep 10
done