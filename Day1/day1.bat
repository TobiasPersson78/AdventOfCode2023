@echo off
cd /D %~dp0
pwsh.exe -ExecutionPolicy RemoteSigned day1.ps1

IF NOT "%TERM_PROGRAM%"=="vscode" pause
