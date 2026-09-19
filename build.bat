@echo off
echo Building Luxury Clock V2...

set MSBUILD="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"

if not exist %MSBUILD% (
    echo MSBuild not found. Please ensure .NET Framework is installed.
    pause
    exit /b 1
)

%MSBUILD% LuxuryClock.csproj /p:Configuration=Release

if %errorlevel% neq 0 (
    echo.
    echo Build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo Building Setup Installer...
set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
%CSC% /nologo /out:bin\Release\LuxuryClockSetup.exe /target:winexe /win32icon:icon.ico /res:bin\Release\LuxuryClock.exe,Setup.LuxuryClock.exe /res:icon.ico,Setup.icon.ico /r:System.Windows.Forms.dll /r:System.Drawing.dll SetupWizard.cs

if %errorlevel% neq 0 (
    echo.
    echo Setup build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo Build succeeded! Setup Executable is located in bin\Release\LuxuryClockSetup.exe
pause
