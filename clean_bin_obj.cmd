@echo off
color e
echo Removes bin and obj folders
rd /s /q ConsoleTest\bin
rd /s /q ConsoleTest\obj
rd /s /q SwtorUnleashed\bin
rd /s /q SwtorUnleashed\obj
del /f /q /a:h SwtorUnleashed.v12.suo
pause