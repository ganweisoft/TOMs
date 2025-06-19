<p align="left" dir="auto">
  <a href="https://opensource.ganweicloud.com" rel="nofollow">
    <img width="130" height="130" style="max-width:100%;" src="https://github.com/ganweisoft/.github/blob/main/images/logo.png">
  </a>
</p>

[![GitHub license](https://camo.githubusercontent.com/5eaf3ed8a7e8ccb15c21d967b8635ac79e8b1865da3a5ccf78d2572a3e10738a/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f646f746e65742f6173706e6574636f72653f636f6c6f723d253233306230267374796c653d666c61742d737175617265)](https://github.com/ganweisoft/TOMs/blob/main/LICENSE) [![Build Status](https://github.com/ganweisoft/TOMs/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ganweisoft/TOMs/actions) ![](https://img.shields.io/badge/join-discord-infomational)

简体中文 | [English](README.md)

TOMs 是一个完全开源、体系化、插件化、高性能、开箱即用、可用于生产环境，是物联网行业应用的一站式开发框架。TOMs 核心代码采用C#编写，基于最新版本的.NET 9.0构建，其扩展出来的插件可以采用Java、 Python 、C++、Go、Rust等主流语言编写。TOMs 支持容器部署、本地部署（Windows、Linux、MacOS）、端侧低功耗设备部署（ARM、RISC-V）。

##  目录

1. [简介](#1-简介)
2. [框架概述](#2-框架概述)
   - 2.1 [核心特性](#21-核心特性)
   - 2.2 [技术矩阵](#22-技术矩阵)
3. [必备工具](#3-必备工具)
   - 3.1 [操作系统支持](#31-操作系统支持)
   - 3.2 [.NET 9.0 运行时安装](#32-net-90-运行时安装)
   - 3.3 [克隆仓库](#33-克隆仓库)
4. [构建](#4-构建)
   - 4.1 [在Linux上构建](#41-在linux上构建)
   - 4.2 [在macOS上构建](#42-在macos上构建)
   - 4.3 [在Windows上构建](#43-在windows上构建)
5. [打包](#5-打包)
6. [安装与运行](#6-安装与运行)
   - 6.1 [Linux系统安装](#61-linux-系统上安装)
   - 6.2 [容器化部署](#62-容器上安装)
   - 6.3 [Windows系统安装](#63-windows-系统上安装)
7. [源码构建说明](#7-源码构建说明)
8. [License](#8-license)
9. [测试](#9-测试)
10. [发布](#10-发布)
11. [覆盖率](#11-覆盖率)
12. [贡献指南](#12-如何提交贡献)



# 1. 简介
TOMs（Thing-Oriented Middleware System）是一款面向物联网领域的**企业级开源开发框架**，基于MIT协议，为工业互联网、智慧城市等场景提供**全生命周期解决方案**。框架采用模块化架构设计，集成设备接入、协议转换、边缘计算及应用开发能力，支持从端侧嵌入式设备到云端服务的完整技术栈覆盖。

核心引擎基于.NET 9.0运行时构建，采用C# 12.0实现高性能组件，并通过AOT编译技术优化资源占用。其创新性的**插件化架构**支持gRPC微服务解耦，提供Java/Python/C++/Go/Rust等多语言SDK，实现核心系统与功能插件的动态扩展与热插拔。

<a id="框架概述"></a>
# 2. 框架概述

TOMs 是一款**完全开源**的物联网行业应用一站式开发框架，具备以下核心优势：
-  **体系化架构**：提供完整的物联网解决方案架构
- **插件化设计**：支持多语言插件扩展机制
-  **高性能引擎**：基于.NET 9.0 构建的高效运行时
-  **开箱即用**：内置生产环境级基础组件
-  **全平台覆盖**：支持多样化的部署场景

![](./media/img-CN/framework.svg)

## 2.1 核心特性
| 特性                | 描述                                                                 |
|---------------------|----------------------------------------------------------------------|
| **技术栈**           | 主体采用 C# 开发，基于 .NET 9.0 运行时                              |
| **多语言扩展**       | 支持 Java/Python/C++/Go/Rust 等主流语言开发插件                      |
| **部署灵活性**       | 容器化部署 / 本地部署 / 端侧低功耗设备部署                          |
| **硬件适配**         | 全面支持 ARM/RISC-V 等嵌入式架构                                     |

## 2.2 技术矩阵
```mermaid
graph TD
    A[核心框架] -->|C# .NET 9.0| B[基础服务]
    A -->|多语言网关| C[插件生态]
    B --> D[设备管理]
    B --> E[规则引擎]
    B --> F[数据总线]
    C --> G[Java插件]
    C --> H[Python插件]
    C --> I[C++插件]
    C --> J[Go插件]
    C --> K[Rust插件]
```

# 3. 必备工具

## 3.1 操作系统支持

| 操作系统    | 支持版本                             | 架构              | 备注                      |
| ----------- | ------------------------------------ | ----------------- | ------------------------- |
| **Windows** | Client 7 SP1+, 8.1, 10 1607+         | x64, x86          | Nano Server 支持 ARM32    |
|             | Server 2012 R2+                      |                   |                           |
| **macOS**   | Mac OS X 10.13+                      | x64               |                           |
| **Linux**   | RHEL 6+, CentOS 7/8, Oracle 7/8      | x64               | 微软/红帽/Oracle 支持策略 |
|             | Fedora 30+, Debian 9+, Ubuntu 16.04+ | x64, ARM32, ARM64 | Debian/Ubuntu 需特定版本  |
|             | Linux Mint 18+, openSUSE 15+         | x64               |                           |
|             | SLES 12 SP2+, Alpine 3.8+            | x64, ARM64        | Alpine 支持树莓派3B等设备 |


## 3.2 .NET 9.0 运行时安装 

###  3.2.1 系统要求

| 组件         | 要求说明                                                                 |
|--------------|--------------------------------------------------------------------------|
| **操作系统** | macOS 11+/Windows 10+/Linux (Ubuntu 20.04+/CentOS 7+/Fedora 30+)        |
| **架构**     | x64/ARM64（推荐x64架构）                                                 |
| **内存**     | 最低4GB（推荐8GB+）                                                      |
| **磁盘空间** | 至少2GB可用空间                                                          |

###  3.2.2 安装步骤

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
   ```bash
   wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --channel 9.0 --runtime aspnetcore
   ```

2. **CentOS/RHEL**
   ```bash
   sudo dnf install https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
   sudo dnf install dotnet-sdk-9.0
   ```

## 3.3 克隆仓库

通过如下命令将 TOMs 仓库克隆到指定计算机：

```bash
git clone https://github.com/ganweisoft/TOMs
```
# 4. 构建

## 4.1 在macOS上构建

TOMs 在macOS上构建，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)

## 4.2 在Linux上构建

TOMs 在Linux上构建，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)

## 4.3 在Windows上构建

TOMs 在Windows上构建，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)


# 5. 打包
TOMs 打包，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)


# 6. 安装与运行

## 6.1 Linux 系统上安装

<details>

<summary>Linux 系统上安装详细步骤</summary>

1. 安装运行环境执行<code>./install.sh</code>安装命令，安装程序等待安装。

![](./media/img-CN/9ba2b269dfddd5d0b5f6bdd7b413f344.png)

```sh
sh ./install.sh
```
![](./media/img-CN/f898bc359ef42cbfe8e9fff4e3256389.png)

</details>

### 6.2 容器上安装

<details>

<summary>容器上安装上安装详细步骤</summary>

1.  下载IoTCenter源包。

    -   到下载地址[IoTCenter平台下载中心](https://www.ganweicloud.com/Download)下载最新版本的IoTCenter软件压缩包。
    ![](./media/img-CN/image40.png)

    -   命令
        ```
        curl -o IoTCenter.zip https://ganweicloud.obs.cn-north-4.myhuaweicloud.com/%E6%96%87%E6%A1%A3%E7%BD%91%E7%AB%99/IoTCenter%E7%89%88%E6%9C%AC/6.0.3/6.0.3-Linux_x86_64.zip
        ```

2.  解压源包。

    -   命令
        ```
        unzip IoTCenter.zip
        ```

3.  创建镜像EntryPoint脚本runGW.sh。

    -   代码
        ```
        #!/bin/bash
        umask 027
        # start service
        nohup /opt/ganwei/IoTCenter/IoTCenterWeb/shell/restart.sh >/dev/null 2>&1 &
        
        sleep 15
        echo "start service finish"
        tail -f /dev/null
        
        ```

4.  创建打包Dockerfile。

    -   代码
        ```
        # 基础镜像为微软官方aspnetcore6.0镜像
        FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0
        
        # 初始化时区为中国上海（东八区）
        ENV TZ="Asia/Shanghai"
        
        # 去除apt安装缓存
        RUN apt update \
        && rm -rf /var/lib/apt/lists/*
        
        RUN mkdir -p /opt/ganwei/ \
        && chmod -R 755 /opt/ganwei
        
        COPY IoTCenter /opt/ganwei/IoTCenter
        COPY runGW.sh /opt/ganwei/
        
        # 修改.sh文件的权限，谨防越权
        RUN find /opt/ganwei/IoTCenter/IoTCenterWeb -name *.sh -exec chmod 550 {} \;
        
        EXPOSE 44380
        
        WORKDIR /opt/ganwei/
        
        # 启动EntryPoint脚本
        CMD sh runGW.sh
        ```

5.  部署中也许需要持久化一些文件，如：配置文件AlarmCenterProperties.xml；SQLite数据库文件（使用关系型数据库服务器MySQL等则忽略）；插件安装目录packages等。
    -   容器启动命令
        ```
        docker run -itd \
         -v /var/gwiot/CurveData:/opt/ganwei/IoTCenter/CurveData \
         -v /var/gwiot/database:/opt/ganwei/IoTCenter/database \
         -v /var/gwiot/data:/opt/ganwei/IoTCenter/data/ \
         -v /var/gwiot/packages:/opt/ganwei/IoTCenter/IoTCenterWeb/packages \
         -p 44380:44380 iotcenter:6.1.0
        ```

    -   注意：由于映射目录时，目录文件会自动清除。所以在启动脚本中需做一些前置工作。
        -   在启动时需先生成sqlite数据库文件。
        ```
        DBPATH=/opt/ganwei/IoTCenter/database/Database.db
        
        # Linux系统需安装sqlite3包。
        if [ ! -f "$DBPATH" ]; then
            cat /ganwei/config/sqlite.sql | sqlite3 $DBPATH
            echo "execte initialized sql script"
        fi
        ```
        -   由其他备用目录复制配置文件AlarmCenterProperties.xml至运行目录
        ```
        XMLDIR=/ganwei/data/
        
        cp -rf $XMLDIR/* /opt/ganwei/IoTCenter/data/
        ```

        -   由其他备用目录原始插件包至运行目录下。
        ```
        PKGDIR=/ganwei/packages/
        
        cp -rn /opt/ganwei/IoTCenter/IoTCenterWeb/originpackages/* $PKGDIR
        ```
6.  最后的启动脚本runGW.sh
    ```
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
    ```

</details>



### 6.3 Windows 系统上安装


<details>

<summary>Windows 系统上安装</summary>

1. 解压安装包到D:\ganwei\IoTCenter目录下。

![](./media/img-CN/4637162272eb61bdc78fda67e2b8ff06.png)

2. 使用services\regist.bat脚本注册服务，需要以管理员身份运行。

![](./media/img-CN/8465daf903822d053b375cb7802888a8.png)

![](./media/img-CN/c0fe9712c7adf7dcba4b8e55556d5d2e.png)

3. 在windows下会注册2个windows服务【IoTCenter】【IoTCenterDaemon】，并使用IoTCenterDaemon进行服务保活，该保活程序会每隔10秒钟探测一次前两个服务的运行状态，若停止，会自动拉起。

![](./media/img-CN/083a07b609654b6865b0b36eb04e8c9a.png)

4. windows下强制停止，需首先停止IoTCenterDaemon服务守护进程，再停止IoTCenter。可使用sc stop xxx来停止，或使用windows界面操作。

![](./media/img-CN/a047be24d5a34f835457d026edaa1d87.png)

5. 若需要注销服务，使用services\unregist.bat脚本以管理员身份运行，打开任务管理器查看是否注销成功。

![](./media/img-CN/e15500af0b16f545c20f2b4e566cb5a2.png)

6. 服务启动后，浏览器打开https://127.0.0.1:44380。

![](./media/img-CN/571937ef07667ba205730055934309b9.png)

7. 点击高级按钮，点击继续前往。

![](./media/img-CN/360df3ca93262288b8721e8d33bea258.png)

8. 进入登录页。。用户首次登录是会显示服务条款信息

![](./media/img-CN/2d78462a7caace6bfcd070c83435abc5.png)

![](./media/img-CN/20250411134054.png)

9. 若是非本机访问，请使用ip 进行访问

![](./media/img-CN/336c838df91228e71cf5abf6a9119916.png)

</details>


# 7. 源码构建说明

TOMs 源码构建说明，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)

# 8. License

TOMs 使用非常宽松的MIT协议，请见 [License](https://github.com/ganweisoft/TOMs/blob/main/LICENSE)。

# 9. 测试

TOMs 测试说明，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)

# 10. 发布
TOMs 发布说明，请见[Releases](https://github.com/ganweisoft/TOMs/releases)

# 11. 覆盖率
TOMs 覆盖率，请见[wiki](https://github.com/ganweisoft/TOMs/wiki)

## 12. 如何提交贡献

我们非常欢迎开发者提交贡献, 如果您发现了一个bug或者有一些想法想要交流，欢迎提交一个[issue](https://github.com/ganweisoft/TOMs/blob/main/CONTRIBUTING.md).
