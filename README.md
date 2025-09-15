<p align="center" dir="auto">
  <a href="https://opensource.ganweicloud.com" rel="nofollow">
    <img width="260" height="260" src="/media/img/logo.png">
  </a>
</p>

[![GitHub license](https://camo.githubusercontent.com/5eaf3ed8a7e8ccb15c21d967b8635ac79e8b1865da3a5ccf78d2572a3e10738a/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f646f746e65742f6173706e6574636f72653f636f6c6f723d253233306230267374796c653d666c61742d737175617265)](https://github.com/ganweisoft/TOMs/blob/main/LICENSE) ![GitHub Pages](https://img.shields.io/github/deployments/ganweisoft/TOMs/github-pages?label=GitHub%20Pages) [![Build Status](https://github.com/ganweisoft/TOMs/actions/workflows/build.yml/badge.svg)](https://github.com/ganweisoft/TOMs/actions) ![Docker](https://img.shields.io/github/v/release/ganweisoft/toms?logo=docker) ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white) ![Java](https://img.shields.io/badge/Java-ED8B00?logo=openjdk&logoColor=white) ![Python](https://img.shields.io/badge/Python-3776AB?logo=python&logoColor=white) ![C++](https://img.shields.io/badge/C%2B%2B-00599C?logo=c%2B%2B&logoColor=white) ![Rust](https://img.shields.io/badge/Rust-000000?logo=rust&logoColor=white) ![Go](https://img.shields.io/badge/Go-00ADD8?logo=go&logoColor=white) ![](https://img.shields.io/badge/join-discord-infomational)

English | [简体中文](README-CN.md)

# 1. Introduction

**TOMs** is a fully open-source, high-performance, systematic, plugin-oriented, and general-purpose development framework designed for various intelligent scenarios. **TOMs** is built on the latest **.NET 9.0**, and its extension plugins can be developed using programming languages such as **C#**, **Java**, **Python**, **C++**, **Go**, and **Rust**. **TOMs** supports cloud-native deployment, local deployment (**Windows**, **Linux**, **macOS**), and embedded device deployment.

## 1.1 The Meaning of TOMs
|Letter  | Explanation |
|----|---------------|
|**T**| Translation，Converts heterogeneous external data into a unified data model |
|**O**| Orchestrator，Allows customizable orchestration of data processing, business workflows, and UI interfaces|
|**M**| Module，All features are plugin-based, supporting free installation and uninstallation |
|**s**| Plural form (s)，Developers can contribute various plugins and enable online transactions, gathering contributions to build a thriving ecosystem |

## 1.2 Framework Overview

![](./media/img/architecture.design.en.png)

## 1.3 Application Scenarios

**TOMs** can be widely applied to various large-scale intelligent scenarios, including but not limited to **Industrial**、**Transportation**、**Port**、**Electricity**、**Construction**、**Agriculture**、**Data Centers**、**New Energy**、**Environmental Protection**、**Smart Cities**.

![应用场景](/media/img/application-scenarios-en.jpg)

## 1.4 Software Interface
[![Software Interface Cover](/media/img/software-interface-cover.gif)](https://ganweisoft.github.io/TOMs/docs/video-software-interface.html)

💡 Tip: Click to watch video online

## 1.5 Related Sub-Repositories

📦 Core Components
| Badge | Repository | Description | Status |
|------------|------------|-------------|--------|
|<img src="https://raw.githubusercontent.com/ganweisoft/Gateway/main/GWDataCenter/logo.jpg" width="80" alt="Gateway Logo">| [Gateway](https://github.com/ganweisoft/Gateway) | high-performance, centralized communication and scheduling module for various device plugins. It uniformly converts heterogeneous data into standardized models and delivers core functionalities such as real-time data storage, alarm triggering, linkage control, and task planning | <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" /> |
|<img src="https://raw.githubusercontent.com/ganweisoft/GrpcServer/main/src/logo.jpg" width="80" alt="GrpcServer Logo">| [GrpcServer](https://github.com/ganweisoft/GrpcServer) | Builds a lightweight, high-performance proxy service framework using the gRPC (Google Remote Procedure Call) protocol. It models communication interfaces using the Protocol Buffers (protobuf) interface definition language, and supports cross-language and cross-platform service integration and invocation |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" /> |
|<img src="https://raw.githubusercontent.com/ganweisoft/WebPlugins/main/src/logo.jpg" width="80" alt="GrpcServer Logo">| [WebPlugins](https://github.com/ganweisoft/WebPlugins) | A modular and pluggable application framework based on ASP.NET Core and VUE. Built on the design principles of loose coupling and high cohesion, it provides an extensible and maintainable application framework. By completely decoupling core logic from functional components, it enables secondary development |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" />


🛠️ Tools & Utilities
| Badge | Repository | Description | Status |
|------------|------------|-------------|--------|
|<img src="https://raw.githubusercontent.com/ganweisoft/Devices/main/src/src/logo.jpg" width="80" alt="GrpcServer Logo">| [Devices](https://github.com/ganweisoft/Devices) | Natively support Modbus and OPC UA (Open Platform Communications Unified Architecture), two of the most widely used communication protocols in the field of industrial automation, providing efficient and reliable data acquisition and device interaction capabilities |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" width="200" height="20" />


🌐 Multi-Language
| Badge | Repository | Description | Status |
|------------|------------|-------------|--------|
|<img src="https://img.shields.io/badge/-Python-3776AB?style=flat&logo=python&logoColor=white" width="67" height="15" alt="Python Logo">| [Gateway4Python](https://github.com/ganweisoft/Gateway4Python) | Distributed Gateway for Python | <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" /> |
|<img src="https://img.shields.io/badge/-Java-ED8B00?style=flat&logo=java&logoColor=white" width="67" height="17" alt="Java Logo">| [Gateway4Java](https://github.com/ganweisoft/Gateway4Java) | Distributed Gateway for Java |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" /> |
|<img src="https://img.shields.io/badge/-.NET-512BD4?style=flat&logo=.net&logoColor=white" width="67" height="15" alt="CSharp Logo">| [Gateway4CSharp](https://github.com/ganweisoft/Gateway4CSharp) | Distributed Gateway for .NET |  <img src="https://img.shields.io/badge/status-active-brightgreen" alt="Status" />

# 2. Prerequisites

## 2.1 Supported OS

| OS          | Supported Versions                     | Architectures      | Notes                      |
| ----------- | ------------------------------------ | ----------------- | ------------------------- |
| **Windows** | Windows 10, Windows 11, Windows Server 2012+          | x86/x64/Arm64          | Nano Server is supported in Windows Server 2025, 2019 and 2022    |
| **Linux**   | OpenEuler, Kylin, OpenKylin, Deepin, UOS, and other mainstream domestic operating systems      | x64/Arm64               |  |
|             | Ubuntu(25.04, 24.04, 22.04), Debian 12, RHEL(10, 9, 8), CentOS (10, 9), Azure Linux 3.0, Fedora(42, 41), OpenSUSE Leap 15.6, SUSE Enterprise Linux 15.6, Alpine(3.22, 3.21, 3.20, 3.19) | x64/Arm64 |  |
| **macOS**   | macOS 13(Ventura), macOS 14(Sonoma), macOS 15(Sequoia)                      | x64/Arm64               |                           |

💡Note: As of June 30, 2024, all versions of CentOS Linux have reached end-of-life (EOL). It is recommended to migrate to alternative operating systems.

## 2.2 .NET 9.0 Runtime Installation 

### 2.2.1 System Suggests 

| Component   | Explanation                                                          |
|-------------|-----------------------------------------------------------------------|
| **OS**      | Windows 11 / Linux (Ubuntu 22.04+, Debian 12, Fedora 40+, etc.) / Domestic Linux OS (e.g., OpenEuler, Kylin, Deepin) / macOS 13+     |
| **Arch**    | x64/Arm64 (recommended to use 64-bit architecture)                                          |
| **Memory**  | Minimum 1GB (recommended to use 8GB+)                                       |
| **Storage** | At least 500M free space                                              |

### 2.2.2 Installation Steps

#### macOS

1. **Homebrew Installation** (recommended)
   ```bash
   brew install --cask dotnet-sdk
   ```

2. **Manual Download**
   - Visit [Microsoft Download Center](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Download **`.NET 9.0 Runtime (macOS x64/ARM64 Installer)`**
   - Double-click installer and follow prompts

#### Windows

1. **Installer Method**
   1. Visit [Microsoft Download Center](https://dotnet.microsoft.com/download/dotnet/9.0)
   2. Download **`.NET 9.0 Runtime (Windows x64/ARM64 Installer)`**
   3. Run installer, check **`I accept the license terms`**, click **`Install`**

2. **Command Line Method**
   ```cmd
   # Run PowerShell as Administrator
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

## 2.3 Clone the repo

Clone the TOMs repository using:
```bash
git clone https://github.com/ganweisoft/TOMs
```

# 3. Build & Release

## 3.1 Local Build
Local build script (Windows / Linux)
```bash
windows-linux.bat
```
See [Window-Linux Build Script](https://github.com/ganweisoft/TOMs/blob/main/windows-linux.bat)

Local build script (Docker)
```bash
docker.bat
```
See [Docker Build Script](https://github.com/ganweisoft/TOMs/blob/main/docker.bat)

## 3.2 Cloud Build

Automated cloud builds(Windows / Linux / Docker) using [GitHub Actions](https://docs.github.com/en/actions), see [Cloud Build Script](https://github.com/ganweisoft/TOMs/blob/main/.github/workflows/build.yml)

# 4. Installation & Execution

## 4.1 Windows Installation
Run installation with:
```bash
regist.bat
```
💡Note: Requires Administrator privileges on Windows

## 4.2 Linux Installation
Run installation with:
```bash
./install.sh
```

## 4.3 Docker Installation
Run installation with:
```bash
docker run -d -p 44380:44380 -p 44381:44381 --name toms ghcr.io/ganweisoft/toms:latest
```

# 5. License

TOMs uses the permissive MIT License, see [LICENSE](https://github.com/ganweisoft/TOMs/blob/main/LICENSE)

# 6. Release Notes

## 6.1 Windows / Linux Releases

See Windows and Linux release history at [Releases](https://github.com/ganweisoft/TOMs/releases)

## 6.2 Docker Releases

See Docker release history at [Releases](https://github.com/ganweisoft/TOMs/pkgs/container/toms/versions)

# 7. Contributing

We welcome contributions! If you find a bug or have ideas to discuss, please submit an [issue](https://github.com/ganweisoft/TOMs/blob/main/CONTRIBUTING.md)
