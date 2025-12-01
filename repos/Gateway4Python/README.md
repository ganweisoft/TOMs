[![GitHub license](https://camo.githubusercontent.com/5eaf3ed8a7e8ccb15c21d967b8635ac79e8b1865da3a5ccf78d2572a3e10738a/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f646f746e65742f6173706e6574636f72653f636f6c6f723d253233306230267374796c653d666c61742d737175617265)](https://github.com/ganweisoft/Mini-Gateway-Python/blob/main/LICENSE) ![Docker](https://img.shields.io/github/v/release/ganweisoft/toms?logo=docker) ![Python](https://img.shields.io/badge/Python-3776AB?logo=python&logoColor=white) ![](https://img.shields.io/badge/join-discord-infomational)


## 介绍

分离自[**GateWay**](https://github.com/ganweisoft/Gateway)的一个可扩展的Python环境分布式网关, 使得开发者可以使用自己熟悉的开发语言进行快速开发

### 消息路径

使用[dapr](https://docs.dapr.io/)的消息通道, 传输设备的实时值到GateWay主网关. 架构图:   
![img.png](img.png)

1. GateWay主网关作为subscriber. 
2. 消息队列中间件可选MQTT, Kafka, Redis等.
3. GateWay4Python作为publisher.

### 内部扩展

1. 同GateWay一样, 可进行内部扩展. 只需继承CEquipBase, 将类名命名为CEquip.
生成的文件放入GWHost1的上层目录的dll目录下.
GWMiniDataCenter启动是即可自扫描加载.
```python
if __name__ == '__main__':
    from ganweisoft.DataCenter import DataCenter

    DataCenter.start()
```
