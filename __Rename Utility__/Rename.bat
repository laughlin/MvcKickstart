@echo off

set /p projectName="New Project Name: " %=%

@echo Renaming 'KickstartTemplate' to '%projectName%'
::@echo %CD%
cd ../KickstartTemplate/KickstartTemplate
call:renameFiles
@echo Renaming 'KickstartTemplate.Tests' to '%projectName%.Tests'
::@echo %CD%
cd ../../KickstartTemplate/KickstartTemplate.Tests
call:renameFiles
cd ../
::@echo %CD%
@echo Renaming root files
::@echo %CD%
::find/replace in root directory (no recursion to avoid the __Rename Utility__ directory)
"../__Rename Utility__/fart.exe" -- * "KickstartTemplate" %projectName%

::rename files in root directory (no recursion to avoid the __Rename Utility__ directory)
"../__Rename Utility__/fart.exe" -f -- * "KickstartTemplate" %projectName%

@echo Renaming 'Directories' from %CD%
::need this for renaming directories
ren KickstartTemplate %projectName%
ren KickstartTemplate.Tests %projectName%.Tests
cd ../
@echo Renaming root directory
ren KickstartTemplate %projectName%

@echo.
@echo Done and done!!
pause

::pause
goto:eof
:: ----- Functions -----

:renameFiles
::echo %CD%
::find/replace in project subdirectories
"../../__Rename Utility__/fart.exe" -r -- * "KickstartTemplate" %projectName%

::rename files in project subdirectories
"../../__Rename Utility__/fart.exe" -r -f -- * "KickstartTemplate" %projectName%

goto:eof