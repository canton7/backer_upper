@echo off
FOR /F "tokens=1 delims=," %%i IN ('schtasks /FO CSV ^| FIND "\BackerUpper"') DO (schtasks /Delete /TN %%i /F)