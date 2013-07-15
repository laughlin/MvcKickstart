@echo off

set /p projectName="New Project Name: " %=%

@echo Renaming 'MvcKickstart' to '%projectName%'
cd ../MvcKickstart/MvcKickstart
call:renameFiles
cd ../MvcKickstart/MvcKickstart.Tests
call:renameFiles

cd ../
::@echo %CD%
::find/replace in root directory (no recursion to avoid the __Rename Utility__ directory)
"__Rename Utility__/fart.exe" -- * "MvcKickstart" %projectName%

::rename files in root directory (no recursion to avoid the __Rename Utility__ directory)
"__Rename Utility__/fart.exe" -f -- * "MvcKickstart" %projectName%

::need this for renaming directories
ren MvcKickstart %projectName%
ren MvcKickstart.Tests %projectName%.Tests
@echo.
@echo Done and done!!
pause

::pause
goto:eof
:: ----- Functions -----

:renameFiles
::echo %CD%
::find/replace in project subdirectories
"../__Rename Utility__/fart.exe" -r -- * "MvcKickstart" %projectName%

::rename files in project subdirectories
"../__Rename Utility__/fart.exe" -r -f -- * "MvcKickstart" %projectName%

goto:eof