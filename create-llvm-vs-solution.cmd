@echo off
setlocal enabledelayedexpansion

:: Start in the directory containing this script
cd %~dp0

:: Determine platform RID and build folder
call tooling\determine-rid.cmd || exit /B !ERRORLEVEL!
echo Creating Visual Studio solution for %PLATFORM_RID%...
set BUILD_FOLDER=bin\llvm\%PLATFORM_RID%-vs

if "%PLATFORM_RID%" == "win-x64" (
    set CMAKE_ARCHITECTURE=x64
) else if "%PLATFORM_RID%" == "win-arm64" (
    set CMAKE_ARCHITECTURE=ARM64
) else if "%PLATFORM_RID%" == "win-x86" (
    set CMAKE_ARCHITECTURE=Win32
) else if "%PLATFORM_RID%" == "win-arm" (
    set CMAKE_ARCHITECTURE=ARM
) else (
    echo "RID '%PLATFORM_RID%' is not supported by this script." 1>&1
)

:: Ensure build folder is protected from Directory.Build.* influences
if not exist %BUILD_FOLDER% (
    mkdir %BUILD_FOLDER% 2>NUL
    echo ^<Project^>^</Project^> > %BUILD_FOLDER%/Directory.Build.props
    echo ^<Project^>^</Project^> > %BUILD_FOLDER%/Directory.Build.targets
    echo # > %BUILD_FOLDER%/Directory.Build.rsp
)

:: Generate Visual Studio solution
cmake -G "Visual Studio 17 2022" -A %CMAKE_ARCHITECTURE% -Thost=x64 -S external\llvm-project\llvm\ -B %BUILD_FOLDER% ^
    -DCMAKE_BUILD_TYPE=RelWithDebInfo ^
    -DLLVM_ENABLE_PROJECTS=clang ^
    -DLLVM_INCLUDE_TESTS=off ^
    -DLLVM_INCLUDE_BENCHMARKS=off
