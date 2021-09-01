@echo off
cmake -G "Visual Studio 16 2019" -A x64 -Thost=x64 -S external/llvm-project/llvm/ -B build-llvm-vs -DCMAKE_BUILD_TYPE=RelWithDebInfo -DLLVM_ENABLE_PROJECTS=clang -DLLVM_INCLUDE_TESTS=off -DLLVM_INCLUDE_BENCHMARKS=off
echo ^<Project^>^</Project^> > build-llvm-vs/Directory.Build.props
echo ^<Project^>^</Project^> > build-llvm-vs/Directory.Build.targets
echo # > build-llvm-vs/Directory.Build.rsp
