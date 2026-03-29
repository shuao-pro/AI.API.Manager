@echo off
echo ========================================
echo    AI.API.Manager Quick Start
echo ========================================

where powershell >nul 2>nul
if %errorlevel% neq 0 (
    echo [ERROR] PowerShell not found. Please install PowerShell 5.1 or later.
    pause
    exit /b 1
)

echo [INFO] Starting PowerShell script...
powershell -ExecutionPolicy Bypass -File "%~dp0start.ps1" %*

if %errorlevel% neq 0 (
    echo [ERROR] Start script failed, error code: %errorlevel%
    pause
)