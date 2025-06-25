<p align="left" dir="auto">
  <a href="https://opensource.ganweicloud.com" rel="nofollow">
    <img width="130" height="130" style="max-width:100%;" src="https://github.com/ganweisoft/.github/blob/main/images/logo.png">
  </a>
</p>

[![GitHub license](https://camo.githubusercontent.com/5eaf3ed8a7e8ccb15c21d967b8635ac79e8b1865da3a5ccf78d2572a3e10738a/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f646f746e65742f6173706e6574636f72653f636f6c6f723d253233306230267374796c653d666c61742d737175617265)](https://github.com/ganweisoft/TOMs/blob/main/LICENSE) [![Build Status](https://github.com/ganweisoft/TOMs/actions/workflows/build.yml/badge.svg)](https://github.com/ganweisoft/TOMs/actions) ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white) ![Java](https://img.shields.io/badge/Java-ED8B00?logo=openjdk&logoColor=white) ![Python](https://img.shields.io/badge/Python-3776AB?logo=python&logoColor=white) ![C++](https://img.shields.io/badge/C%2B%2B-00599C?logo=c%2B%2B&logoColor=white) ![Rust](https://img.shields.io/badge/Rust-000000?logo=rust&logoColor=white) ![Go](https://img.shields.io/badge/Go-00ADD8?logo=go&logoColor=white) ![Docker](https://img.shields.io/badge/-Docker-2496ED?style=flat&logo=docker&logoColor=white) ![Kubernetes](https://img.shields.io/badge/Kubernetes-326CE5?logo=kubernetes&logoColor=white) ![](https://img.shields.io/badge/join-discord-infomational)

简体中文 | [English](README.md)

**TOMs** 是一个完全开源、高性能、体系化、插件化、面向各种智慧化场景的通用开发框架。**TOMs** 基于最新的 **.NET 9.0** 构建，其扩展插件可以使用 **C#**、 **Java**、**Python**、**C++**、**Go** 和 **Rust** 等编程语言进行开发。**TOMs** 支持云原生部署、本地部署（**Windows**、**Linux**、**macOS**）和嵌入式设备部署。

**TOMs**的含义
|字母  | 解释 |
|----|---------------|
|**T**| Translation，把各种外部异构数据转化为统一的数据模型 |
|**O**| Orchestrator，数据处理、业务流程以及UI界面可自定义编排|
|**M**| Module，所有功能全部插件化，可以自由安装卸载 |
|**s**| 复数，开发者可以贡献各类插件并实现在线交易，聚沙成塔，共筑生态 |

**TOMs**可以广泛应用于各类重量级的智慧化场景，比如**工业**、**交通**、**港口**、**电力**、**建筑**、**农业**、**数据机房**、**新能源**、**环保**、**城市**等。

![应用场景](/media/img/application-scenarios.jpg)

## 软件界面

##  目录

1. [框架概述](#1-框架概述)
   - 1.1 [核心特性](#11-核心特性)
2. [必备工具](#2-必备工具)
   - 2.1 [操作系统支持](#21-操作系统支持)
   - 2.2 [.NET 9.0 运行时安装](#22-net-90-运行时安装)
   - 2.3 [克隆仓库](#23-克隆仓库)
3. [构建发布](#3-构建发布)
   - 3.1 [本地构建](#31-本地构建)
   - 3.2 [云端构建](#32-云端构建)
4. [安装与运行](#4-安装与运行)
   - 4.1 [Linux系统安装](#41-Linux-系统上安装运行)
   - 4.2 [Windows系统安装](#42-Windows-系统上安装运行)
5. [License](#5-license)
6. [测试](#6-测试)
7. [发布](#7-发布)
8. [子仓库列表](#8-子仓库列表)
9. [贡献指南](#9-如何提交贡献)

<a id="框架概述"></a>
# 1. 框架概述

![](./media/img-CN/architecture.design.cn.png)

## 1.1 核心特性
| 特性                | 描述                                                                 |
|---------------------|----------------------------------------------------------------------|
| **技术栈**           | 主体采用 C# 开发，基于 .NET 9.0 运行时                              |
| **多语言扩展**       | 支持 Java/Python/C++/Go/Rust 等主流语言开发插件                      |
| **部署灵活性**       | 云原生部署 / 本地部署 / 嵌入式设备部署                          |
| **硬件适配**         | 全面支持 ARM/RISC-V 等嵌入式架构                                     |

# 2. 必备工具

## 2.1 操作系统支持

| 操作系统    | 支持版本                             | 架构              | 备注                      |
| ----------- | ------------------------------------ | ----------------- | ------------------------- |
| **Windows** | Windows 10, Windows 11, Windows Server 2012+          | x86/x64/Arm64          | Nano Server 支持(2019,2022)    |
| **Linux**   | OpenEuler(华为欧拉), Kylin(银河麒麟), OpenKylin(开放麒麟), Deepin(深度), UOS(统信)等国产操作系统      | x64/Arm64               |  |
|             | Ubuntu 22.04+, Debian 12, RHEL(8,9), CentOS 9, Fedora(40,41), openSUSE Leap(15.5,15,6), SUSE Enterprise Linux(15.5,15,6), Alpine 3.19+ | x64/Arm64 |  |
| **macOS**   | macOS 13(Ventura), macOS 14(Sonoma), macOS 15(Sequoia)                      | Arm64               |                           |

💡注意：截至2024年6月30日，CentOS Linux所有版本已全部停止维护，建议使用其他操作系统替代


## 2.2 .NET 9.0 运行时安装 

###  2.2.1 系统建议

| 组件         | 说明                                                                 |
|--------------|--------------------------------------------------------------------------|
| **操作系统** | Windows 11/Linux (Ubuntu 22.04+/Debian 12/Fedora 40+等)/Linux国产操作系统/macOS 13+        |
| **架构**     | x64/ARM64（推荐x64架构）                                                 |
| **内存**     | 最低4GB（推荐8GB+）                                                      |
| **磁盘空间** | 至少2GB可用空间                                                          |

###  2.2.2 安装步骤

#### macOS

1. **通过Homebrew安装**（推荐）
   ```bash
   brew install --cask dotnet-sdk
   ```

2. **手动下载**
   - 访问[微软官方下载中心](https://dotnet.microsoft.com/download/dotnet/9.0)
   - 下载 **`.NET 9.0 Runtime (macOS x64/ARM64 Installer)`**
   - 双击安装包，按提示完成安装

#### Windows

1. **通过安装包安装**
   1. 访问[微软官方下载中心](https://dotnet.microsoft.com/download/dotnet/9.0)
   2. 下载 **`.NET 9.0 Runtime (Windows x64/ARM64 Installer)`**
   3. 双击安装包，勾选 **`I accept the license terms`**，点击 **`Install`**

2. **通过命令行安装**
   ```cmd
   # 以管理员身份运行PowerShell
   Start-Process -FilePath "dotnet-runtime-9.0.x-win-x64.exe" -ArgumentList "/quiet /norestart" -Wait
   ```

#### Linux

1. **Ubuntu/Debian**
 ```shell
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-9.0
```

2. **CentOS/RHEL**
```shell
sudo rpm -Uvh https://packages.microsoft.com/config/centos/$(rpm -E %centos)/packages-microsoft-prod.rpm
sudo yum install aspnetcore-runtime-9.0
```

## 2.3 克隆仓库

通过如下命令将 TOMs 仓库克隆到指定计算机：

```bash
git clone https://github.com/ganweisoft/TOMs
```
# 3. 构建发布

## 3.1 本地构建
TOMs 可以通过以下命令进行本地构建：
```bash
TOMs.build.bat
```
请见 [本地构建脚本](https://github.com/ganweisoft/TOMs/blob/main/TOMs.build.bat)

## 3.2 云端构建
使用[Github Actions](https://docs.github.com/zh/actions)进行云端自动化构建，请见 [云端构建脚本](https://github.com/ganweisoft/TOMs/blob/main/.github/workflows/build.yml)

# 4. 安装与运行

## 4.1 Linux 系统上安装运行

TOMs 可以通过以下命令进行安装运行：
```bash
sh install.sh
```
### 4.2 Windows 系统上安装运行
TOMs 可以通过以下命令进行安装运行：
```bash
regist.bat
```

💡注意：Windows上运行安装脚本需要以管理员身份运行。

# 5. License

TOMs 使用非常宽松的MIT协议，请见 [License](https://github.com/ganweisoft/TOMs/blob/main/LICENSE)。

# 6. 测试

TOMs 测试说明，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)

# 7. 发布
TOMs 发布说明，请见[Releases](https://github.com/ganweisoft/TOMs/releases)

# 8. 子仓库列表
📦 核心仓库
| 徽章 | 仓库 | 描述 | 状态 |
|------------|------------|-------------|--------|
|<img src="https://raw.githubusercontent.com/ganweisoft/Gateway/main/GWDataCenter/logo.jpg" width="80" alt="Gateway Logo">| [Gateway](https://github.com/ganweisoft/Gateway) | 高性能的、各类设备插件的集中通讯调度模块，把各类异构数据统一转换为标准模型，完成实时数据的存储、报警、联动、任务规划等核心功能 |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" /> |
|<img src="https://raw.githubusercontent.com/ganweisoft/GrpcServer/main/src/logo.jpg" width="80" alt="GrpcServer Logo">| [GrpcServer](https://github.com/ganweisoft/GrpcServer) | 采用 gRPC（Google Remote Procedure Call）协议 构建轻量级、高性能的代理服务框架，基于 Protocol Buffers（protobuf） 接口定义语言进行通信接口建模，支持跨语言、跨平台的服务集成与调用 |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" /> |
|<img src="https://raw.githubusercontent.com/ganweisoft/WebPlugins/main/src/logo.jpg" width="80" alt="GrpcServer Logo">| [WebPlugins](https://github.com/ganweisoft/WebPlugins) | 基于ASP.NET Core和VUE的模块化和插件化应用程序框架，基于松耦合、高内聚的设计理念，构建了一个可扩展、易维护的应用框架，通过将核心逻辑与功能组件完全解耦，可进行二次开发 |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" />

🛠️ 工具仓库
| 徽章 | 仓库 | 描述 | 状态 |
|------------|------------|-------------|--------|
|<img src="https://raw.githubusercontent.com/ganweisoft/Devices/main/src/src/logo.jpg" width="80" alt="GrpcServer Logo">| [Devices](https://github.com/ganweisoft/Devices) | 原生支持 Modbus 与 OPC UA（Open Platform Communications Unified Architecture） 两种工业自动化领域主流通信协议，提供高效、可靠的数据采集与设备交互能力。 |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" />

# 9. 如何提交贡献

我们非常欢迎开发者提交贡献, 如果您发现了一个bug或者有一些想法想要交流，欢迎提交一个[issue](https://github.com/ganweisoft/TOMs/blob/main/CONTRIBUTING.md).
