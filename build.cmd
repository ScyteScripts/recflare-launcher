@echo off
setlocal
cd /d "%~dp0"
call npm run package
if errorlevel 1 exit /b 1
call npm test
if errorlevel 1 exit /b 1
call npm run test:managed
exit /b %errorlevel%
