#!/usr/bin/env pwsh
<#
.SYNOPSIS
    AI.API.Manager 一键启动脚本
.DESCRIPTION
    启动 AI.API.Manager 项目，包括数据库和 API 服务。
    如果 Docker 和 Docker Compose 可用，则使用 docker-compose 启动容器。
    否则，使用 dotnet run 启动本地开发服务器（需要本地 SQL Server）。
.PARAMETER Mode
    启动模式：'docker' 或 'local'，默认自动检测。
.PARAMETER Build
    是否重新构建 Docker 镜像（仅 Docker 模式）。
.PARAMETER Detach
    是否在后台运行容器（仅 Docker 模式）。
.EXAMPLE
    .\start.ps1
    自动检测并启动服务。
.EXAMPLE
    .\start.ps1 -Mode docker -Build
    使用 Docker 模式并重新构建镜像。
.EXAMPLE
    .\start.ps1 -Mode local
    使用本地开发模式。
#>

param(
    [ValidateSet('docker', 'local')]
    [string]$Mode = 'auto',
    [switch]$Build,
    [switch]$Detach
)

$ErrorActionPreference = "Stop"

# 颜色定义
$Green = "`e[32m"
$Yellow = "`e[33m"
$Red = "`e[31m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-Info {
    param([string]$Message)
    Write-Host "$Green[INFO]$Reset $Message"
}

function Write-Warn {
    param([string]$Message)
    Write-Host "$Yellow[WARN]$Reset $Message"
}

function Write-Error {
    param([string]$Message)
    Write-Host "$Red[ERROR]$Reset $Message"
}

function Write-Step {
    param([string]$Message)
    Write-Host "$Blue[STEP]$Reset $Message"
}

function Test-Command {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    } catch {
        return $false
    }
}

function Start-DockerMode {
    Write-Step "使用 Docker 模式启动服务..."

    if ($Build) {
        Write-Info "重新构建 Docker 镜像..."
        docker-compose build
    }

    $detachFlag = ""
    if ($Detach) {
        $detachFlag = "-d"
        Write-Info "将在后台运行容器..."
    }

    Write-Info "启动容器服务..."
    docker-compose up $detachFlag
}

function Start-LocalMode {
    Write-Step "使用本地开发模式启动服务..."

    # 检查 .NET SDK
    if (-not (Test-Command "dotnet")) {
        Write-Error "未找到 .NET SDK。请安装 .NET 8 SDK 或更高版本。"
        exit 1
    }

    # 检查 SQL Server 连接（可选）
    Write-Warn "本地模式需要 SQL Server 实例正在运行。"
    Write-Warn "请确保已更新 AI.API.Manager.API/appsettings.json 中的连接字符串。"

    # 应用数据库迁移
    Write-Step "应用数据库迁移..."
    try {
        dotnet ef database update --project AI.API.Manager.Infrastructure --startup-project AI.API.Manager.API
    } catch {
        Write-Error "数据库迁移失败：$_"
        Write-Warn "请确保 SQL Server 正在运行且连接字符串正确。"
        exit 1
    }

    # 启动 API
    Write-Step "启动 API 服务..."
    Write-Info "API 将在 https://localhost:5001 和 http://localhost:5000 运行"
    Write-Info "Swagger UI: https://localhost:5001/swagger"
    Write-Info "按 Ctrl+C 停止服务"

    Set-Location AI.API.Manager.API
    dotnet run
}

# 主逻辑
Write-Host "$Green========================================$Reset"
Write-Host "$Green    AI.API.Manager 一键启动脚本       $Reset"
Write-Host "$Green========================================$Reset"

# 检测模式
if ($Mode -eq 'auto') {
    Write-Info "自动检测运行环境..."
    if (Test-Command "docker-compose" -and Test-Command "docker") {
        $Mode = 'docker'
        Write-Info "检测到 Docker 和 Docker Compose，使用 Docker 模式。"
    } else {
        $Mode = 'local'
        Write-Warn "未检测到 Docker 或 Docker Compose，使用本地模式。"
    }
}

# 根据模式启动
switch ($Mode) {
    'docker' {
        if (-not (Test-Command "docker-compose")) {
            Write-Error "Docker Compose 未安装。请安装 Docker Desktop。"
            exit 1
        }
        if (-not (Test-Command "docker")) {
            Write-Error "Docker 未安装。请安装 Docker Desktop。"
            exit 1
        }
        Start-DockerMode
    }
    'local' {
        Start-LocalMode
    }
    default {
        Write-Error "未知模式：$Mode"
        exit 1
    }
}