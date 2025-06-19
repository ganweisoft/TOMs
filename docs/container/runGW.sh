#!/bin/bash
umask 027

PKGDIR=/ganwei/packages/
XMLDIR=/ganwei/data/
DBPATH=/opt/ganwei/IoTCenter/database/Database.db

# create database
if [ ! -f "$DBPATH" ]; then
    cat /ganwei/config/sqlite.sql | sqlite3 $DBPATH
    echo "execte initialized sql script"
fi

# copy xml
cp -rf $XMLDIR/* /opt/ganwei/IoTCenter/data/

# copy packages
cp -rn /opt/ganwei/IoTCenter/IoTCenterWeb/originpackages/* $PKGDIR
    
# start service
nohup /opt/ganwei/IoTCenter/IoTCenterWeb/shell/restart.sh >/dev/null 2>&1 &

sleep 15
echo "start service finish"
tail -f /dev/null