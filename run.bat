@echo off
echo Starting API...
cd AI.API.Manager.API
dotnet run --urls "http://localhost:5000;https://localhost:5001"
pause