#!/bin/bash

# AI.API.Manager 一键启动脚本
# 启动 AI.API.Manager 项目，包括数据库和 API 服务。
# 如果 Docker 和 Docker Compose 可用，则使用 docker-compose 启动容器。
# 否则，使用 dotnet run 启动本地开发服务器（需要本地 SQL Server）。

set -e

# 颜色定义
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

function print_info {
    echo -e "${GREEN}[INFO]${NC} $1"
}

function print_warn {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

function print_error {
    echo -e "${RED}[ERROR]${NC} $1"
}

function print_step {
    echo -e "${BLUE}[STEP]${NC} $1"
}

function test_command {
    command -v "$1" >/dev/null 2>&1
}

function start_docker_mode {
    print_step "使用 Docker 模式启动服务..."

    if [ "$BUILD" = true ]; then
        print_info "重新构建 Docker 镜像..."
        docker-compose build
    fi

    local detach_flag=""
    if [ "$DETACH" = true ]; then
        detach_flag="-d"
        print_info "将在后台运行容器..."
    fi

    print_info "启动容器服务..."
    docker-compose up $detach_flag
}

function start_local_mode {
    print_step "使用本地开发模式启动服务..."

    # 检查 .NET SDK
    if ! test_command "dotnet"; then
        print_error "未找到 .NET SDK。请安装 .NET 8 SDK 或更高版本。"
        exit 1
    fi

    # 检查 SQL Server 连接（可选）
    print_warn "本地模式需要 SQL Server 实例正在运行。"
    print_warn "请确保已更新 AI.API.Manager.API/appsettings.json 中的连接字符串。"

    # 应用数据库迁移
    print_step "应用数据库迁移..."
    if ! dotnet ef database update --project AI.API.Manager.Infrastructure --startup-project AI.API.Manager.API; then
        print_error "数据库迁移失败。"
        print_warn "请确保 SQL Server 正在运行且连接字符串正确。"
        exit 1
    fi

    # 启动 API
    print_step "启动 API 服务..."
    print_info "API 将在 https://localhost:5001 和 http://localhost:5000 运行"
    print_info "Swagger UI: https://localhost:5001/swagger"
    print_info "按 Ctrl+C 停止服务"

    cd AI.API.Manager.API
    dotnet run
}

# 解析参数
MODE="auto"
BUILD=false
DETACH=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -m|--mode)
            MODE="$2"
            shift 2
            ;;
        -b|--build)
            BUILD=true
            shift
            ;;
        -d|--detach)
            DETACH=true
            shift
            ;;
        *)
            print_error "未知参数: $1"
            exit 1
            ;;
    esac
done

# 主逻辑
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}    AI.API.Manager 一键启动脚本       ${NC}"
echo -e "${GREEN}========================================${NC}"

# 检测模式
if [ "$MODE" = "auto" ]; then
    print_info "自动检测运行环境..."
    if test_command "docker-compose" && test_command "docker"; then
        MODE="docker"
        print_info "检测到 Docker 和 Docker Compose，使用 Docker 模式。"
    else
        MODE="local"
        print_warn "未检测到 Docker 或 Docker Compose，使用本地模式。"
    fi
fi

# 根据模式启动
case "$MODE" in
    "docker")
        if ! test_command "docker-compose"; then
            print_error "Docker Compose 未安装。请安装 Docker Desktop。"
            exit 1
        fi
        if ! test_command "docker"; then
            print_error "Docker 未安装。请安装 Docker Desktop。"
            exit 1
        fi
        start_docker_mode
        ;;
    "local")
        start_local_mode
        ;;
    *)
        print_error "未知模式: $MODE"
        exit 1
        ;;
esac