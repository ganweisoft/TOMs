#!/bin/bash

# 初始化日志文件
LOGS_DIR="build_logs.txt"
RELEASE_DIR="./Release"
GATEWAY_DIR="$RELEASE_DIR/bin"
WEBAPI_DIR="$RELEASE_DIR/IoTCenterWeb/publish"
WWWROOT_DIR="$WEBAPI_DIR/wwwroot"
NVM_DIR="src/front-end/nvm-setup.sh" # 假设你有离线安装脚本或手动安装逻辑

# 删除旧日志
rm -f "$LOGS_DIR"

# 时间戳函数
get_timestamp() {
    formatted_datetime=$(date "+%Y-%m-%d %H:%M:%S")
}

log() {
    get_timestamp
    echo "[$formatted_datetime] $1" >> "$LOGS_DIR"
    echo "[$formatted_datetime] $1"
}

# 创建/清空发布目录
setup_release_dir() {
    log "删除已生成的构建日志文件"
    log "设置整体发布目录路径: $RELEASE_DIR"
    if [ -d "$RELEASE_DIR" ]; then
        log "发布目录($RELEASE_DIR)已存在，删除中..."
        rm -rf "$RELEASE_DIR"
        if [ $? -ne 0 ]; then
            log "删除发布目录失败！可能有文件被占用或权限不足。"
            exit 1
        fi
        log "发布目录($RELEASE_DIR)已删除"
    fi
    mkdir -p "$RELEASE_DIR"
    log "发布目录($RELEASE_DIR)已创建完成"
}

# 安装 Node.js 并构建前端
install_node_and_build_frontend() {
    log "开始构建前端解决方案..."

    # 检查是否安装了 nvm
    if [ ! -s "$HOME/.nvm/nvm.sh" ]; then
        log "未检测到 nvm，请先安装 nvm：https://github.com/nvm-sh/nvm" 
        exit 1
    fi

    export NVM_DIR="$HOME/.nvm"
    [ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"

    NODE_VERSION1="14.21.3"
    NODE_VERSION2="23.1.0"

    check_install_node() {
        local version=$1
        if nvm ls | grep -q "$version"; then
            log "Node.js v$version 已安装。"
        else
            log "Node.js v$version 未安装，正在安装..."
            NVM_NODEJS_ORG_MIRROR=https://npmmirror.com/mirrors/node 
            nvm install "$version"
            log "Node.js v$version 已完成安装"
        fi
    }

    check_install_node "$NODE_VERSION1"
    check_install_node "$NODE_VERSION2"

    # 使用 Node.js 14 构建插件
    nvm use "$NODE_VERSION1"
    cd src/front-end || { log "进入前端目录失败"; exit 1; }
    node build.js
    log "前端插件已构建完成"

    # 使用 Node.js 23 构建主应用
    nvm use "$NODE_VERSION2"
    if ! command -v pnpm &> /dev/null; then
        npm install -g pnpm --registry=https://registry.npmmirror.com 
        log "pnpm 已完成安装"
    fi

    pnpm install
    pnpm run build
    log "前端项目已构建完成"
}

# 检查 .NET SDK
check_dotnet_sdk() {
    log "正在检测是否已安装.NET SDK"
    dotnet --list-sdks > /dev/null 2>&1
    if [ $? -ne 0 ]; then
        log ".NET SDK 未安装或路径未加入环境变量，请确认安装 https://dotnet.microsoft.com/download/dotnet/9.0" 
        exit 1
    fi

    SDK_VERSION=$(dotnet --version)
    log "当前.NET SDK 版本: $SDK_VERSION"

    if [[ ! "$SDK_VERSION" =~ ^9\. ]]; then
        log "要求安装.NET SDK为9.0版本"
        log "下载地址：https://dotnet.microsoft.com/download/dotnet/9.0" 
        exit 1
    fi
}

# 构建网关
build_gateway() {
    log "开始构建网关解决方案..."

    # 复制配置
    if [ ! -d "$release_dir/data" ]; then
        cp -r src/gateway/config "$RELEASE_DIR/"
    else
        log "数据库配置目录已存在，跳过处理..."
    fi

    cd src/gateway || { log "无法进入网关目录"; exit 1; }
    dotnet restore IoTCenterHost.sln
    dotnet build IoTCenterHost.sln --configuration Debug -o "$GATEWAY_DIR"
    if [ $? -ne 0 ]; then
        log "错误：网关构建失败"
        exit 1
    fi
    log "完成构建网关解决方案"
}

# 构建 WebAPI
build_webapi() {
    log "开始构建 WebApi 解决方案..."

    cd src/webapi || { log "无法进入 WebAPI 目录"; exit 1; }
    dotnet restore IoTCenterWebApi.sln
    dotnet build IoTCenterWebApi.csproj --configuration Debug -o "$WEBAPI_DIR"
    if [ $? -ne 0 ]; then
        log "错误：WebApi 构建失败"
        exit 1
    fi

    # 移动插件目录
    if [ -d "plugins" ]; then
        mv plugins "$WEBAPI_DIR"
    fi

    log "完成构建 WebApi 解决方案"
}

# 启动服务
start_services() {
    log "正在启动网关服务"
    cd "$GATEWAY_DIR" && nohup dotnet GWHost1.dll > gateway.log 2>&1 &
    sleep 2

    log "正在启动 WebApi 服务"
    cd "$WEBAPI_DIR" && nohup dotnet IoTCenterWebApi.dll > webapi.log 2>&1 &
    sleep 2

    log "正在打开访问链接: https://localhost:44380" 
    xdg-open "https://localhost:44380" 
}

# 主流程
setup_release_dir
install_node_and_build_frontend
check_dotnet_sdk
build_gateway
build_webapi
start_services