@ECHO off

SET RSBUILDPATH=C:\Users\donsm_001\Google Drive\Code\client\build

REM **** Make sure the version was passed in ****
IF "%1" == "" GOTO HELP
IF "%1" == "-h" GOTO HELP
IF "%1" == "/h" GOTO HELP
IF "%1" == "/?" GOTO HELP

REM **** Create the NuGet package for Squirrel to use ****
IF NOT EXIST BuildPackages MD BuildPackages
nuget pack Resquirrelly.Debug.nuspec -Version %1 -OutputDirectory BuildPackages
IF NOT ERRORLEVEL 0 (
    ECHO Creating the NuGet package failed.
    GOTO EXIT
)

REM **** Build and sign the installer using the new NuGet package ****

ECHO Attempting to build the installer using Squirrel.
.\packages\squirrel.windows.0.8.3.1\tools\Squirrel.exe --releasify BuildPackages\Resquirrelly.%1.nupkg 
IF NOT ERRORLEVEL 0 (
    ECHO Building the installer failed with an error.
    ECHO The log file at .\packages\squirrel.windows.0.8.3.1\tools\SquirrelSetup.log might help.
    GOTO EXIT
)
ECHO Successfully created Releases\Setup.exe.

REM **** Rename Setup.exe to ResquirrellyInstaller.exe ****

ECHO Renaming the installer to ResquirrellyInstaller.exe
MV Releases\Setup.exe Releases\ResquirrellyInstaller.exe

GOTO DONE

:DONE
ECHO.
ECHO Done.
GOTO EXIT

:HELP
ECHO Usage: release version
ECHO Example: release 1.1.0.25

:EXIT