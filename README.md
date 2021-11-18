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

## Building

Below are instructions for building ClangSharp.Pathogen on Windows, Linux, and macOS. The specific versions listed are not hard requirements and are simply what is generally used in the day-to-day development of ClangSharp.Pathogen. (For example: CMake 3.13 and Visual Studio 2019 should be enough to build LLVM on Windows even though we generally use newer versions.)

### Building on Windows

Windows 10 21H1 x64 is recommended.

#### Windows Prerequisites

Tool | Tested Version
-----|--------------------
[CMake](https://cmake.org/) | 3.22.0
[Ninja](https://ninja-build.org/) | 1.10.0
[Visual Studio](https://visualstudio.microsoft.com/vs/) | 2022 - 17.1.0 preview 1.1
[.NET 5 SDK](http://dot.net/) | 5.0

Visual Studio requires the "Desktop development with C++" and  ".NET desktop development" workloads to be installed.

If [sccache](https://github.com/mozilla/sccache) (0.2.15 recommended) is present on your path it will automatically be used to accellerate subsequent builds.

#### Windows Build Steps

1. Ensure Git submodules are up-to-date with `git submodule update --init --recursive`
2. Run `build-native.cmd` (This will take quite a while the first time.)
3. Open the solution in Visual Studio (`ClangSharp.Pathogen.sln`) and build as normal.

### Building on Linux

Ubuntu 20.04 Focal x64 is recommended for development.

Ubuntu 18.04 Bionic is used for x64 CI builds.

Ubuntu 20.04 Focal is used for ARM64 CI builds.

#### Linux Prerequisites

Package | Tested Version
--------|---------------
`cmake` | 3.16.3
`clang` | 10.0
`libxml2-dev` | 2.9.10
`ninja-build` | 1.10.0
`dotnet-sdk-5.0` | 5.0 ([Via Microsoft](https://docs.microsoft.com/en-us/dotnet/core/install/linux))

If [sccache](https://github.com/mozilla/sccache) (0.2.15 recommended, download the non-dist version) is present on your path it will automatically be used to accellerate subsequent builds.

#### Linux Build Steps

1. Ensure Git submodules are up-to-date with `git submodule update --init --recursive`
2. Run `build-native.sh` (This will take quite a while the first time.)
3. Build the managed components using `dotnet build`

### Building on macOS

macOS support is still experimental. macOS Catalina (10.15.7) x64 is what's generally tested.

#### macOS Prerequisites

Tool | Tested Version
-----|---------------
[CMake](https://cmake.org/) | 3.21.4
[Ninja](https://ninja-build.org/) | 1.10.2
Xcode | 12.4
[.NET 5 SDK](http://dot.net/) | 5.0

If [sccache](https://github.com/mozilla/sccache) (0.2.15 recommended) is present on your path it will automatically be used to accellerate subsequent builds.

Note that CMake, Ninja, and sccache must all be available on your `PATH` when you execute the build script.

Note that the Ninja and sccache binaries are not notarized, see [this comment](https://github.com/ninja-build/ninja/issues/1695#issuecomment-766178554) for a workaround.

#### macOS Build Steps

1. Ensure Git submodules are up-to-date with `git submodule update --init --recursive`
2. Run `build-native.sh` (This will take quite a while the first time.)
3. Build the managed components using `dotnet build`

</details>

## Development

`build-native-debug.cmd` is provided for building a debug build of libclang on Windows. This requires you have the LLVM toolchain (12.0 recommended) installed and `clang-cl` available on your path. (Debug builds of libclang made with MSVC directly tend to have issues.)

If you need to edit the libclang C++ code on Windows, `create-llvm-vs-solution.cmd` is provided to Visual Studio 2022 solution in `bin/llvm/win-x64-vs/`. (Note that 2022 support requires CMake 3.21 or newer. 2019 should work too by modifying the generator used.)

If you're only changing the C# code, you only need to run `build-native.cmd` once (and again whenever there's an update to the `llvm-project` submodule.)

The easiest way to consume an in-development native runtime is to override the ClangSharp native runtime resolver via `LibClangSharpResolver.OverrideNativeRuntime`. In the context of Biohazrd you can also populate the `BIOHAZRD_CUSTOM_LIBCLANG_PATHOGEN_RUNTIME` environment variable with a path to your custom runtime.
