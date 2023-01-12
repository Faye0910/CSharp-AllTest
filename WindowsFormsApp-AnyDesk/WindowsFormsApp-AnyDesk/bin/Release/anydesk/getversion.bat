@echo off
for /f "delims=" %%i in ('"C:\Program Files (x86)\AnyDesk-bbac4dcb\AnyDesk-bbac4dcb.exe" --version') do set CID=%%i 
echo AnyDesk version is: %CID%
pause