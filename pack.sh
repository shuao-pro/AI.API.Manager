#!/bin/bash

# 项目打包脚本
# 创建源代码tar.gz包，排除二进制和临时文件

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_NAME="AI.API.Manager"
OUTPUT_FILE="${PROJECT_NAME}-src-$(date +%Y%m%d-%H%M%S).tar.gz"

echo "项目目录: ${PROJECT_DIR}"
echo "打包项目: ${PROJECT_NAME}"
echo "输出文件: ${OUTPUT_FILE}"

# 使用tar创建压缩包，排除不需要的目录
cd "${PROJECT_DIR}"
tar --exclude='*/bin' \
    --exclude='*/obj' \
    --exclude='coverage-report' \
    --exclude='.git' \
    --exclude='.vs' \
    --exclude='.vscode' \
    --exclude='*.tar.gz' \
    --exclude='pack.sh' \
    --exclude='.gitignore' \
    --ignore-failed-read \
    -czf "${OUTPUT_FILE}" .

echo "打包完成: ${OUTPUT_FILE}"
echo "文件大小: $(du -h "${OUTPUT_FILE}" | cut -f1)"