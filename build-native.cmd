@echo off
setlocal enabledelayedexpansion

:: Initialize Visual Studio dev tools
call vs-tools

:: Initialize cmake (if necessary)
if not exist build\build.ninja (
    set CMAKE_EXTRA_ARGUMENTS=

    :: If sccache is installed, use it
    where sccache 1>NUL 2>NUL
    if not errorlevel 1 (
        echo Found sccache, it will be used to accelerate the build.
        set CMAKE_EXTRA_ARGUMENTS=-DCMAKE_C_COMPILER_LAUNCHER=sccache ^
            -DCMAKE_CXX_COMPILER_LAUNCHER=sccache
    )

    :: Configure
    cmake -G "Ninja" -S external/llvm-project/llvm/ -B build ^
        -DCMAKE_C_COMPILER=cl ^
        -DCMAKE_CXX_COMPILER=cl ^
        -DCMAKE_BUILD_TYPE=Release ^
        -DLLVM_ENABLE_PROJECTS=clang ^
        -DLLVM_INCLUDE_TESTS=off ^
        -DLLVM_INCLUDE_BENCHMARKS=off ^
        !CMAKE_EXTRA_ARGUMENTS!
)

:: Invoke Ninja
ninja -C build libclang
