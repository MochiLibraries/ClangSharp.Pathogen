# ClangSharp + Pathogen Extensions

[![MIT Licensed](https://img.shields.io/github/license/infectedlibraries/clangsharp.pathogen?style=flat-square)](LICENSE.txt)
[![CI Status](https://img.shields.io/github/workflow/status/infectedlibraries/clangsharp.pathogen/ClangSharp.Pathogen?style=flat-square)](https://github.com/InfectedLibraries/ClangSharp.Pathogen/actions?query=workflow%3AClangSharp.Pathogen+branch%3Amain)
[![Sponsor](https://img.shields.io/badge/sponsor-%E2%9D%A4-lightgrey?logo=github&style=flat-square)](https://github.com/sponsors/PathogenDavid)

This repo contains C# bindings for the [libclang Pathogen Extensions](https://github.com/InfectedLibraries/llvm-project) as well as other utilities used by Biohazrd for interacting with ClangSharp.

Currently this project targets libclang 10.0.0 and ClangSharp 10.0.0-beta. Currently the NuGet package only supports Windows x64.

## License

This project is licensed under the MIT License. [See the license file for details](LICENSE.txt).

Additionally, this project has some third-party dependencies. [See the third-party notice listing for details](THIRD-PARTY-NOTICES.md).

## Building

Building is currently only supported on Windows x64 with Visual Studio 2019.

### Prerequisites

Tool | Recommended Version
-----|--------------------
[CMake](https://cmake.org/) | 3.17.2
[Ninja](https://ninja-build.org/) | 1.10.0
[Visual Studio 2019](https://visualstudio.microsoft.com/vs/) | 16.6.4
[.NET Core SDK](http://dot.net/) | 3.1.302

Visual Studio requires the "Desktop development with C++" and  ".NET desktop development" workloads to be installed.

If [sccache](https://github.com/mozilla/sccache) (0.2.12 recommended) is present on your path it will automatically be used to acellerate subsequent builds.

### Build Steps

1. Ensure Git submodules are up-to-date with `git submodule update --init --recursive`
2. Run `build-native.cmd` (This will take quite a while the first time.)
3. Open the solution in Visual Studio (`ClangSharp.Pathogen.sln`) and build/pack as normal.

### Development

If you need to edit the libclang C++ code, it's recommended to run `create-llvm-vs-solution.cmd` to create a Visual Studio solution. (It will be generated in `build-llvm-vs`.) You still need to run `build-native.cmd` to build your changes since ClangSharp.Pathogen only uses its output.

If you're only changing the C# code, you only need to run `build-native.cmd` once (and again whenever there's an update to the `llvm-project` submodule.)
