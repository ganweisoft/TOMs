---
title: Python协议插件开发实践
date: 2025-04-24 09:20:09
permalink: /pages/a173f3/
article: false
editLink: false
---
<CopyCodeComponent/>

## Python协议插件开发

为适应物联网设备多样性及工业协议复杂性，我们选用Python语言开发协议插件。Python凭借其跨平台特性、丰富的协议处理库及动态扩展能力，能够高效适配多设备，构建灵活稳健的物联网系统。其简洁的语法和快速开发特性，进一步降低了协议适配的复杂度。以下是Python开发协议插件的数据流程解析：

## Python开发协议插件数据流程解析

<details class="custom-block details" open> 
<summary>详情</summary>

1. **设备层**  
   各类物联网设备（如传感器、控制器）持续产生运行数据，包括设备状态、环境参数等业务关键信息。Python通过轻量级驱动或SDK与设备交互，支持Modbus、OPC UA、CoAP等工业协议的快速接入。

2. **接入层**  
   通过网络API或串口获取原始数据，Python协议插件利用`pyserial`、`pymodbus`、`aiocoap`等库解析异构协议，将二进制或文本数据转换为JSON/Protobuf等统一格式。动态加载机制支持通过插件目录或配置文件热更新协议解析逻辑。

3. **边缘处理**  
   数据暂存于边缘节点（如Raspberry Pi或工业网关），使用`pandas`进行数据清洗（去噪、插值）、`numpy`进行标准化计算，结合`Pydantic`验证数据结构，确保上传云端前的数据质量。

4. **安全传输**  
   边缘网关通过`paho-mqtt`库实现MQTT协议通信，采用`cryptography`库实现TLS加密传输，支持断线重连与QoS级别配置。Python异步框架（如`asyncio`）可高效处理高并发设备连接。

5. **平台展示**  
   IoTCenter平台接收数据后，通过`Django`/`Flask`后端API提供实时状态接口，前端使用`ECharts`或`Grafana`可视化设备运行趋势。历史数据存储于`TimescaleDB`时序数据库，支持通过`SQLAlchemy`进行快速查询。

![](./media/image30.png)

</details>


### 一、环境配置

<details class="custom-block details" open> 

<summary>详情</summary>

以下是针对 **Windows系统** 的Python开发环境详细配置流程：

1. **下载安装包**：
   - 访问[Python官网](https://www.python.org/downloads/windows/)，下载最新Windows安装包（如 `python-3.x.x.exe`）。
   - **注意**：选择 **Windows installer (64-bit)** 版本（根据系统选择32/64位），**python版本不低于3.11**。

2. **安装步骤**：
   - 双击运行安装包，勾选 **Add Python to PATH**（必须勾选，否则需手动配置环境变量）。
   - 点击 **Install Now** 完成安装。

3. **验证安装**：
   - 按 `Win + R`，输入 `cmd` 打开命令提示符。
   - 输入以下命令检查版本：
     ```cmd
     Python --version
     pip --version
     ```
   - 若提示“命令不存在”，请重新安装并勾选 **Add Python to PATH**或配置环境变量。

4. Windows系统手动配置Python环境变量（可选）

    打开环境变量窗口：
        - 右键 **此电脑** → **属性** → **高级系统设置** → 点击 **环境变量**。

    编辑系统变量Path：
        - 在 **系统变量** 区域，找到并选中 **Path**，点击 **编辑**。

    添加Python路径：
        - 点击 **新建**，输入Python安装路径（如 `C:\Python39`）。
        - 再次点击 **新建**，输入Scripts路径（如 `C:\Python39\Scripts`）。

</details> 

::: warning 注意
您可以使用VS Code、PyCharm、IDLE等开发工具完成下列编码。
输出一个名为xxx.py的文件即可。如CEquip.py
::: 


### 二、随机数代码示例

<details class="custom-block details" open> 

<summary>详情</summary>

```python
import random
import threading

from ganweisoft.Database.YcpTable import YcpTableRow
from ganweisoft.Database.YxpTable import YxpTableRow
from ganweisoft.Logging import Logging
from ganweisoft.interface.CEquipBase import CEquipBase
from ganweisoft.interface import CommunicationState


class CEquip(CEquipBase):
    def __init__(self):
        super().__init__()
        self.data_fetch_counter = 0
        self.lock = threading.Lock()

    def GetData(self, pEquip) -> CommunicationState:
        super().Sleep(1000)
        if super().RunSetParmFlag:
            return CommunicationState.setreturn

        comm_state = super().GetData(pEquip)
        if comm_state != CommunicationState.ok:
            return comm_state

        if not pEquip.GetEvent():
            return CommunicationState.fail
        return CommunicationState.ok

    def GetYC(self, row: YcpTableRow) -> bool:
        min_val = max(row.val_min, 0.0)
        max_val = min(row.val_max, 100.0)
        super().SetYCData(row, random.uniform(min_val, max_val))
        return True

    def GetYX(self, row: YxpTableRow) -> bool:
        yx_value = random.choice([True, False])
        super().SetYXData(row, yx_value)
        return True

    def SetParm(self, main_instruct: str, minor_instruct: str, value: str) -> bool:
        try:
            if main_instruct.lower() == "setycyxvalue":
                if minor_instruct is None or len(minor_instruct) < 3:
                    Logging.write_log_file(f"Invalid MinorInstruct format: {minor_instruct}")
                    return False

                prefix = minor_instruct[0].lower()
                index_str = minor_instruct[2:]
                if not index_str.isdigit():
                    Logging.write_log_file(f"Invalid YC/YX index: {index_str}")
                    return False

                ycyx_no = int(index_str)
                if ycyx_no <= 0:
                    Logging.write_log_file(f"YC/YX index must be > 0: {ycyx_no}")
                    return False

                if prefix == 'c':  # YC
                    if value is None or len(value) < 0:
                        Logging.write_log_file("Missing YC value")
                        return False
                    yc_value = float(value)
                    with self.lock:
                        yc_results = super().YCResults
                        yc_results[ycyx_no] = yc_value
                    return True
                elif prefix == 'x':  # YX
                    yx_value = int(value) > 0
                    with self.lock:
                        yx_results = super().YXResults
                        yx_results[ycyx_no] = yx_value
                    return True
            return False
        except ValueError as e:
            Logging.write_log_file(f"Number format error: {str(e)}")
            return False
        except Exception as e:
            Logging.write_log_file(f"SetParm error: {str(e)}")
            return False

if __name__ == '__main__':
    from ganweisoft.DataCenter import DataCenter
    DataCenter.start()
```
</details>

### 三、调试运行

<details class="custom-block details" open> 

<summary>详情</summary>

1、配置变量导入


**可选：1、使用配置文件。下载方式见下文：IoTCenter 边缘网关部署 -> 应用管理 -> ./config文件夹下**

此时的目录结构为：

**目录结构：**
```
.
│─CEquip.py
│
└─config
        config.properties
```

**可选：2、使用环境变量，此时可使用set（windows）或者export（Linux）先初始化后，再调用运行命令**

```
set InstanceId=20250304
set MqUsername=gateway
set MqPassword=********
set MqServer=127.0.0.1
set MqPort=1883
```

2、在xxx.py同级目录下执行命令（windows环境）

```
python xxx.py
```
![alt text](./media/image28.png)


### 打包

1、打包为独立可执行文件

::: warning 注意
Windows和Linux下打包出来的文件是不相同的，所以在不同操作系统环境中，需独立打包
::: 


```cmd
# 安装pyinstaller
pip install pyinstaller

# 打包CEquip.py成CEquip执行文件
pyinstaller CEquip.py --onefile --hidden-import=gw-mini-datacenter
```

2、打包Docker镜像包

将第一步生成的文件打包成Docker镜像包

```Dockerfile
# using debian bookworm image
FROM debian:bookworm-slim

# Continue with application deployment
RUN mkdir /opt/ganwei
COPY IoTCenter.Python /opt/ganwei/IoTCenter.Python

WORKDIR /opt/ganwei/IoTCenter.Python/

CMD ["./CEquip"]
```

最后输出的文件为一个tar包如下图

![alt text](./media/image29.png)

</details>

## **IoTCenter 边缘网关部署**

### 一、必备插件安装

<details class="custom-block details" open> 

<summary>详情</summary>

1、边缘网关平台子设备插件

2、边缘网关平台扩展服务

3、边缘网关管理

安装完成上述插件后重启平台。

![image-20250409153811981](./media/image-20250409153811981.png)

![alt text](./media/image27.png)

</details>

### 二、添加边缘应用

<details class="custom-block details" open> 

<summary>详情</summary>

1、编辑平台信息，MQTT服务填写本地ip端口默认为1883

2、点击测试查看是否连接成功。

![image-20250409154447068](./media/image-20250409154447068.png)

![image-20250409154606130](./media/image-20250409154606130.png)

</details>

### 三、网关注册

<details class="custom-block details" open> 

<summary>详情</summary>

1、添加网关（网关名称请不要用中文字符）

2、点击安装

3、复制命令到linux环境并回车

4、下载中等待安装完成

5、安装成功后刷新页面。状态会改为已安装。

![image-20250410111528637](./media/image-3.png)

![image-20250410111528637](./media/image-4.png)

![image-20250410111528637](./media/image-5.png)

![image-20250409162757592](./media/image-20250409162757592.png)

![image-20250409162817759](./media/image-20250409162817759.png)

![image-20250410111528637](./media/image-6.png)

</details>

### 四、应用管理

<details class="custom-block details" open> 

<summary>详情</summary>

1、添加应用

![image-20250410111528637](./media/image-7.png)

给应用添加版本，参数参考如下截图，当前版本只支持容器部署。

![image-20250410111528637](./media/image-8.png)

![image-20250410111528637](./media/image-9.png)

![image-20250410111528637](./media/image-10.png)

![image-20250410111528637](./media/image-11.png)

![image-20250410111528637](./media/image-12.png)

完成上述步骤后该应用状态为**运行中**

</details>

### 五、数据同步

<details class="custom-block details" open> 

<summary>详情</summary>

1、 配置设备的通讯端口（下级实例标识）和设备属性（PYTHON|BCDataSimu.STD），设备驱动库：GW.MqttGatewayServer.Subscribe.STD.dll
注：BCDataSimu.STD 为协议插件名

![image-20250410111528637](./media/image-15.png)


2、同步设备

![image-20250410111528637](./media/image-16.png)

![image-20250410111528637](./media/image-17.png)

3、查看实时值

![image-20250410111528637](./media/image-18.png)

</details>

## 常见问题

<details class="custom-block details" open> 

<summary>详情</summary>

1. 数据为***
检查下列必备插件插件是否均为最新版本、设备配置是否正常。

- 边缘网关管理数据接收服务

- 边缘网关管理协议插件

- 边缘网关管理


</details>