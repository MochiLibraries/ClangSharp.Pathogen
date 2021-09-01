# ClangSharp + Pathogen Extensions

[![MIT Licensed](https://img.shields.io/github/license/infectedlibraries/clangsharp.pathogen?style=flat-square)](LICENSE.txt)
[![CI Status](https://img.shields.io/github/workflow/status/infectedlibraries/clangsharp.pathogen/ClangSharp.Pathogen?style=flat-square)](https://github.com/InfectedLibraries/ClangSharp.Pathogen/actions?query=workflow%3AClangSharp.Pathogen+branch%3Amain)
[![NuGet Version](https://img.shields.io/nuget/v/ClangSharp.Pathogen?style=flat-square)](https://www.nuget.org/packages/ClangSharp.Pathogen/)
[![Sponsor](https://img.shields.io/badge/sponsor-%E2%9D%A4-lightgrey?logo=github&style=flat-square)](https://github.com/sponsors/PathogenDavid)

This repo contains C# bindings for the [libclang Pathogen Extensions](https://github.com/InfectedLibraries/llvm-project) as well as other utilities used by Biohazrd for interacting with ClangSharp.

Currently this project targets libclang 12.0.1 and ClangSharp 12.0.0-beta2. The NuGet package is currently built for Windows x64 and Linux x64 (glibc >= 2.27).

## License

This project is licensed under the MIT License. [See the license file for details](LICENSE.txt).

Additionally, this project has some third-party dependencies. [See the third-party notice listing for details](THIRD-PARTY-NOTICES.md).

## Building - Windows

Windows 10 20H2 x64 is recommended.

### Prerequisites

Tool | Tested Version
-----|--------------------
[CMake](https://cmake.org/) | 3.18.4
[Ninja](https://ninja-build.org/) | 1.10.0
[Visual Studio 2019](https://visualstudio.microsoft.com/vs/) | 16.8.5
[.NET Core SDK](http://dot.net/) | 5.0

Visual Studio requires the "Desktop development with C++" and  ".NET desktop development" workloads to be installed.

If [sccache](https://github.com/mozilla/sccache) (0.2.15 recommended) is present on your path it will automatically be used to accellerate subsequent builds.

### Build Steps

1. Ensure Git submodules are up-to-date with `git submodule update --init --recursive`
2. Run `build-native.cmd` (This will take quite a while the first time.)
3. Open the solution in Visual Studio (`ClangSharp.Pathogen.sln`) and build/pack as normal.

## Building - Linux

Ubuntu 20.04 Focal x64 is recommended for development, Ubuntu 18.04 Bionic x64 is used for CI builds.

### Prerequisites

Package | Tested Version
--------|---------------
`cmake` | 3.16.3
`clang` | 10.0
`libxml2-dev` | 2.9.10
`ninja-build` | 1.10.0
`dotnet-sdk-5.0` | 5.0 ([Via Microsoft](https://docs.microsoft.com/en-us/dotnet/core/install/linux))

If [sccache](https://github.com/mozilla/sccache) (0.2.15 recommended, download the non-dist version) is present on your path it will automatically be used to accellerate subsequent builds.

### Build Steps

1. Ensure Git submodules are up-to-date with `git submodule update --init --recursive`
2. Run `build-native.sh` (This will take quite a while the first time.)
3. Build the managed components using `dotnet build`
4. If desired, pack the NuGet packages using `dotnet pack`

## Development

If you need to edit the libclang C++ code on Windows, `create-llvm-vs-solution.cmd` is provided for your convenience to create a Visual Studio solution. (It will be generated in `build-llvm-vs`.) You still need to run `build-native.cmd` to build your changes since ClangSharp.Pathogen only uses its output.

`build-native-debug.cmd` is provided for building a debug build of the native runtime on Windows. This requires you have the LLVM toolchain (10.0 recommended) installed and `clang-cl` available on your path.

If you're only changing the C# code, you only need to run `build-native.cmd` once (and again whenever there's an update to the `llvm-project` submodule.)

The easiest way to consume an in-development native runtime is to override the ClangSharp native runtime resolver via `LibClangSharpResolver.OverrideNativeRuntime`.
