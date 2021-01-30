@echo off
setlocal enabledelayedexpansion

:: Initialize Visual Studio dev tools
call tooling\vs-tools

:: Initialize cmake (if necessary)
if not exist build-debug\build.ninja (
    set CMAKE_EXTRA_ARGUMENTS=

    :: If sccache is installed, use it
    where sccache 1>NUL 2>NUL
    if not errorlevel 1 (
        echo Found sccache, it will be used to accelerate the build.
        set CMAKE_EXTRA_ARGUMENTS=-DCMAKE_C_COMPILER_LAUNCHER=sccache ^
            -DCMAKE_CXX_COMPILER_LAUNCHER=sccache
    )

    :: Configure
    cmake -G "Ninja" -S external/llvm-project/llvm/ -B build-debug ^
        -DCMAKE_C_COMPILER=clang-cl ^
        -DCMAKE_CXX_COMPILER=clang-cl ^
        -DCMAKE_BUILD_TYPE=Debug ^
        -DLLVM_ENABLE_PROJECTS=clang ^
        -DLLVM_INCLUDE_TESTS=off ^
        -DLLVM_INCLUDE_BENCHMARKS=off ^
        !CMAKE_EXTRA_ARGUMENTS!
)

:: Invoke Ninja
ninja -C build-debug libclang
