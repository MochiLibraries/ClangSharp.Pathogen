using ClangSharp.Interop;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace ClangSharp.Pathogen
{
    /// <summary>Handles resolving the native runtimes for ClangSharp and ClangSharp.Pathogen.</summary>
    /// <remarks>
    /// For the reasons described in https://github.com/MochiLibraries/llvm-project/issues/2#issuecomment-712897834 we use the same (modified) libclang DLL for
    /// libclang, libClangSharp, and the ClangSharp.Pathogen native runtime. This class manages redirecting the library references within ClangSharp to this central instance.
    ///
    /// Additionally, this class allows you to override the native runtime used for all of the above as well as verify this resolver was used to resolve libclang for ClangSharp.
    /// </remarks>
    public static class LibClangSharpResolver
    {
        private static volatile bool OurResolverWasUsedForClangSharp = false;
        private static IntPtr NativeRuntimeHandle = IntPtr.Zero;

        static LibClangSharpResolver()
        {
            DllImportResolver resolver = Resolver;
            clang.ResolveLibrary += resolver;
            NativeLibrary.SetDllImportResolver(typeof(LibClangSharpResolver).Assembly, resolver);
        }

        /// <summary>Attempts to explicitly load the ClangSharp.Pathogen native runtime and reports whether it was loaded successfully.</summary>
        /// <exception cref="InvalidOperationException">A different thread loaded a different native runtime while we were checking.</exception>
        public static bool TryLoadExplicitly()
        {
            if (NativeRuntimeHandle != IntPtr.Zero)
            { return true; }

            if (!NativeLibrary.TryLoad(PathogenExtensions.LibraryFileName, typeof(LibClangSharpResolver).Assembly, searchPath: null, out IntPtr nativeRuntimeHandle))
            { return false; }

            IntPtr oldHandle = Interlocked.CompareExchange(ref NativeRuntimeHandle, nativeRuntimeHandle, IntPtr.Zero);
            if (oldHandle == IntPtr.Zero || NativeRuntimeHandle == nativeRuntimeHandle)
            { return true; }
            else
            { throw new InvalidOperationException("A different native libclang-pathogen runtime was loaded by a different thread!"); }
        }

        /// <summary>Explicitly installs the ClangSharp.Pathogen native runtime resolver</summary>
        /// <remarks>You generally do not need to call this method</remarks>
        public static void ExplicitlyInstallResolver()
            // (Calling this function implicitly causes the static constructor to be called, which will install our resolver.)
            => VerifyResolverWasUsed();

        /// <summary>Overrides the libclang native runtime to resolve to the specified path.</summary>
        /// <exception cref="InvalidOperationException">The native runtime has already been loaded.</exception>
        public static void OverrideNativeRuntime(string pathToNativeRuntime)
            => OverrideNativeRuntime(NativeLibrary.Load(pathToNativeRuntime));

        /// <summary>Overrides the libclang native runtime to resolve to the specified native library handle.</summary>
        /// <exception cref="InvalidOperationException">The native runtime has already been loaded.</exception>
        public static void OverrideNativeRuntime(IntPtr nativeRuntimeHandle)
        {
            if (NativeRuntimeHandle == nativeRuntimeHandle)
            { return; }

            if (Interlocked.CompareExchange(ref NativeRuntimeHandle, nativeRuntimeHandle, IntPtr.Zero) != IntPtr.Zero)
            { throw new InvalidOperationException("The native libclang-pathogen runtime has already previously been loaded or overridden."); }
        }

        /// <summary>Verifies that the runtime used our resolver to resolve ClangSharp's native runtimes</summary>
        /// <remarks>If this method is used, it should be called before any ClangSharp or ClangSharp.Pathogen methods or types are used. Otherwise the heuristic may be incorrect.</remarks>
        public static unsafe void VerifyResolverWasUsed()
        {
            if (OurResolverWasUsedForClangSharp)
            { return; }

            // Calling createIndex causes the runtime to resolve the export for it.
            // Since it is basically the starting point for actually using libclang, we can use this to determine if ClangSharp was used before our resolver was installed.
            // We can't use something like clang.getVersion because the runtime resolves the DLL separately for each function and it might not have been called.
            void* index = null;
            try
            {
                index = clang.createIndex(0, 0);

                if (!OurResolverWasUsedForClangSharp)
                {
                    throw new InvalidOperationException
                    (
                        "ClangSharp was initialized before the ClangSharp.Pathogen native runtime resolver was installed! " +
                        $"Manually call {typeof(LibClangSharpResolver).FullName}.{nameof(ExplicitlyInstallResolver)} at the start of Main to resolve this issue."
                    );
                }
            }
            finally
            {
                // This needs to happen _after_ the check or the resolving of disposeIndex might erroneously validate our check since someone may have created and index but is yet to dispose of it.
                if (index is not null)
                { clang.disposeIndex(index); }
            }
        }

        private static IntPtr Resolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            switch (Path.GetFileNameWithoutExtension(libraryName))
            {
                case "libclang":
                case "libClangSharp":
                    OurResolverWasUsedForClangSharp = true;
                    goto case PathogenExtensions.LibraryFileName;
                case PathogenExtensions.LibraryFileName:
                {
                    if (searchPath is not null)
                    { Trace.WriteLine($"Non-null {nameof(DllImportSearchPath)} requested by P/Invoke to {libraryName} in {assembly.FullName} was ignored by {typeof(LibClangSharpResolver).FullName}."); }

                    // If we don't have a handle already, load libclang-pathogen using the runtime's default resolver
                    // (We do not want to use NativeLibrary.Load(string) since it does not search for platform-specific native runtimes.)
                    if (NativeRuntimeHandle == IntPtr.Zero)
                    {
                        IntPtr handle = NativeLibrary.Load(PathogenExtensions.LibraryFileName, typeof(LibClangSharpResolver).Assembly, null);
                        Interlocked.CompareExchange(ref NativeRuntimeHandle, handle, IntPtr.Zero);
                    }

                    return NativeRuntimeHandle;
                }
                default:
                    return IntPtr.Zero;
            }
        }
    }
}
