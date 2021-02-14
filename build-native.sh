#!/usr/bin/env bash

# Initialize cmake (if necessary)
if [[ ! -f "build-linux/build.ninja" ]]; then
    CMAKE_EXTRA_ARGUMENTS=""

    # If sccache is installed, use it
    if [[ -x "$(command -v sccache)" ]]; then
        echo "Found sccache, it will be used to accelerate the build."
        CMAKE_EXTRA_ARGUMENTS="-DCMAKE_C_COMPILER_LAUNCHER=sccache \
            -DCMAKE_CXX_COMPILER_LAUNCHER=sccache"
    fi

    # Configure
    cmake -G "Ninja" -S external/llvm-project/llvm/ -B build-linux \
        -DCMAKE_C_COMPILER=clang \
        -DCMAKE_CXX_COMPILER=clang++ \
        -DCMAKE_BUILD_TYPE=Release \
        -DLLVM_ENABLE_PROJECTS=clang \
        -DLLVM_INCLUDE_TESTS=off \
        -DLLVM_INCLUDE_BENCHMARKS=off \
        $CMAKE_EXTRA_ARGUMENTS
fi

# Invoke Ninja
ninja -C build-linux libclang
