@echo off

set /p projectName="New Project Name: " %=%

@echo Renaming 'KickstartTemplate' to '%projectName%'
cd ../KickstartTemplate/KickstartTemplate
call:renameFiles
cd ../KickstartTemplate/KickstartTemplate.Tests
call:renameFiles

cd ../
::@echo %CD%
::find/replace in root directory (no recursion to avoid the __Rename Utility__ directory)
"__Rename Utility__/fart.exe" -- * "KickstartTemplate" %projectName%

::rename files in root directory (no recursion to avoid the __Rename Utility__ directory)
"__Rename Utility__/fart.exe" -f -- * "KickstartTemplate" %projectName%

::need this for renaming directories
ren KickstartTemplate %projectName%
ren KickstartTemplate.Tests %projectName%.Tests
@echo.
@echo Done and done!!
pause

::pause
goto:eof
:: ----- Functions -----

:renameFiles
::echo %CD%
::find/replace in project subdirectories
"../__Rename Utility__/fart.exe" -r -- * "KickstartTemplate" %projectName%

::rename files in project subdirectories
"../__Rename Utility__/fart.exe" -r -f -- * "KickstartTemplate" %projectName%

goto:eof