@echo off
cd %~dp0API
start http://localhost:5018/swagger/index.html
dotnet run
