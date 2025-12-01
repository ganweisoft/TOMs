#!/bin/bash

# 生成随机数文件
#cd /root

#openssl rand -writerand .rnd

# 切换到当前程序目录
#cd "${0%/*}"

# 创建ssl目录,切换到ssl目录
mkdir SSL

# 切换到ssl目录
cd SSL
 
# 生成rsa密钥
openssl genrsa -out private-key.pem 3072

# 生成公钥
openssl rsa -in private-key.pem -pubout -out public-key.pem

# 创建自签名证书
openssl req -subj "/CN=ganweisoft/C=CN/ST=ShenZhen/L=GuangDong/O=customer" -new -x509 -key private-key.pem -out cert.pem -days 3650

# 自动生成随机密码
password=""
echo "start to generate password"
while :
do
	password=$(openssl rand -base64 32|tr -dc [:alnum:][:graph:] |cut -c 1-10)
	#密码长度是否为8位以上（包含8位）
	strlen=`echo $password | grep -E --color '^(.{8,}).*$'`
	if [ ${#strlen} -lt 8 ];then
		echo "the length of password less than 8, should recreate!"
		continue
	fi
	#密码是否有小写字母
	strlow=`echo $password | grep -E --color '^(.*[a-z]+).*$'`
	lowCount=$([ -z "${strlow}" ] && echo 0 || echo 1)
	#密码是否有大写字母
	strupp=`echo $password | grep -E --color '^(.*[A-Z]).*$'`
	upCount=$([ -z "${strupp}" ] && echo 0 || echo 1)
	#密码是否有特殊字符
	strtsc=`echo $password | grep -E --color '^(.*\W).*$'`
	scCount=$([ -z "${strtsc}" ] && echo 0 || echo 1)
	#密码是否有数字
	strnum=`echo $password | grep -E --color '^(.*[0-9]).*$'`
	numCount=$([ -z "${strnum}" ] && echo 0 || echo 1)
	#存在两种以上字符，中断循环
	if [ `expr $lowCount + $upCount + $scCount + $numCount` -ge 2 ];then
		break
	fi
done
echo "success generate password"

# 存储随机生成密钥
echo $password > crf.p

# 将pem转换为pfx
openssl pkcs12 -export -inkey private-key.pem -in cert.pem -out ssl.pfx  -password pass:"${password}"

echo "success generate pfx"

# 删除公钥、私钥文件和证书pem
rm -rf private-key.pem public-key.pem cert.pem

echo "success delete pem file"