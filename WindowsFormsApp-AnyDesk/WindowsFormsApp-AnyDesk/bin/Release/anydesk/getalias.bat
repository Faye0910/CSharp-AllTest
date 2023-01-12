@echo off
for /f "delims=" %%i in ('"C:\Program Files (x86)\AnyDesk-bbac4dcb\AnyDesk-bbac4dcb.exe" --get-alias') do set CID=%%i 
echo AnyDesk Alias is: %CID%
pause