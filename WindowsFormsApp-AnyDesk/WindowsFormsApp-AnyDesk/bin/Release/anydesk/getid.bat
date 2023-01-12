@echo off
for /f "delims=" %%i in ('"C:\Program Files (x86)\AnyDesk-bbac4dcb\AnyDesk-bbac4dcb.exe" --get-id') do set ID=%%i 
echo AnyDesk ID is: %ID%
pause