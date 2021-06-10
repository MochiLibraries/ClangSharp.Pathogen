using ClangSharp.Interop;
using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    /// <summary>A handle to Clang's code generation infrastructure.</summary>
    /// <remarks>Clang's code generation infrastructure is not thread-safe! You should not use the same code generator from multiple threads at the same time.</remarks>
    public sealed class PathogenCodeGenerator : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct RawCodeGenerator
        {
            private readonly IntPtr LlvmContext;
            private readonly IntPtr CodeGenerator;
        }

        private RawCodeGenerator _Handle;
        private bool IsDisposed = false;

        public PathogenCodeGenerator(CXTranslationUnit translationUnit)
        {
            PathogenExtensions.pathogen_CreateCodeGenerator(translationUnit, out _Handle);
        }

        public PathogenCodeGenerator(TranslationUnit translationUnit)
            : this(translationUnit.Handle)
        { }

        internal ref readonly RawCodeGenerator Handle
        {
            get
            {
                if (IsDisposed)
                { throw new ObjectDisposedException(nameof(PathogenCodeGenerator)); }

                return ref _Handle;
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            { return; }

            GC.SuppressFinalize(this);
            IsDisposed = true;
            PathogenExtensions.pathogen_DisposeCodeGenerator(ref _Handle);
        }

        ~PathogenCodeGenerator()
            => Dispose();
    }
}
