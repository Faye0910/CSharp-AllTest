@echo off
for /f "delims=" %%i in ('"C:\Program Files (x86)\AnyDesk-bbac4dcb\AnyDesk-bbac4dcb.exe" --get-status') do set STATUS=%%i 
echo AnyDesk status is: %STATUS%
pause