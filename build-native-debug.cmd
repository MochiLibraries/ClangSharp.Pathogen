@echo off
setlocal enabledelayedexpansion

:: Start in the directory containing this script
cd %~dp0

:: Determine platform RID and build folder
call tooling\determine-rid.cmd || exit /B !ERRORLEVEL!
echo Building libclang for %PLATFORM_RID%...
set BUILD_FOLDER=bin\llvm\%PLATFORM_RID%-debug

:: Initialize Visual Studio dev tools
call tooling\vs-tools || exit /B !ERRORLEVEL!

:: Initialize cmake (if necessary)
if not exist %BUILD_FOLDER%\build.ninja (
    set CMAKE_EXTRA_ARGUMENTS=

    :: If sccache is installed, use it
    where sccache 1>NUL 2>NUL
    if not errorlevel 1 (
        echo Found sccache, it will be used to accelerate the build.
        set CMAKE_EXTRA_ARGUMENTS=-DCMAKE_C_COMPILER_LAUNCHER=sccache ^
            -DCMAKE_CXX_COMPILER_LAUNCHER=sccache
    )

    :: Configure
    cmake -G "Ninja" -S external/llvm-project/llvm/ -B %BUILD_FOLDER% ^
        -DCMAKE_C_COMPILER=clang-cl ^
        -DCMAKE_CXX_COMPILER=clang-cl ^
        -DCMAKE_BUILD_TYPE=Debug ^
        -DLLVM_ENABLE_PROJECTS=clang ^
        -DLLVM_INCLUDE_TESTS=off ^
        -DLLVM_INCLUDE_BENCHMARKS=off ^
        !CMAKE_EXTRA_ARGUMENTS!
)

:: Invoke Ninja
ninja -C %BUILD_FOLDER% libclang
