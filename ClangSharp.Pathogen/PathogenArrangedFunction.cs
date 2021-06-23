using ClangSharp.Interop;
using System;
using System.Runtime.CompilerServices;

namespace ClangSharp.Pathogen
{
    public unsafe class PathogenArrangedFunction : IDisposable
    {
        internal readonly struct RawArrangedFunction
        {
            public readonly PathogenLlvmCallingConventionKind CallingConvention;
            public readonly PathogenLlvmCallingConventionKind EffectiveCallingConvention;
            public readonly PathogenClangCallingConventionKind AstCallingConvention;
            public readonly PathogenArrangedFunctionFlags Flags;
            public readonly uint RequiredArgumentCount;
            public readonly uint ArgumentsPassedInRegisterCount;
            public readonly uint ArgumentCount;
            public readonly PathogenArgumentInfo ReturnInfo;
        }

        private RawArrangedFunction* Handle;

        public PathogenLlvmCallingConventionKind CallingConvention => Handle->CallingConvention;
        public PathogenLlvmCallingConventionKind EffectiveCallingConvention => Handle->EffectiveCallingConvention;
        public PathogenClangCallingConventionKind AstCallingConvention => Handle->AstCallingConvention;
        public PathogenArrangedFunctionFlags Flags => Handle->Flags;
        public uint RequiredArgumentCount => Handle->RequiredArgumentCount;
        public uint ArgumentsPassedInRegisterCount => Handle->ArgumentsPassedInRegisterCount;
        public uint ArgumentCount => Handle->ArgumentCount;
        public ref readonly PathogenArgumentInfo ReturnInfo => ref Handle->ReturnInfo;

        public ArgumentAccessor Arguments => new(Handle, checked((int)ArgumentCount));

        public PathogenArrangedFunction(PathogenCodeGenerator codeGenerator, CXCursor cursor)
        {
            Handle = PathogenExtensions.pathogen_GetArrangedFunction(codeGenerator.Handle, cursor);

            if (Handle is null)
            { throw new InvalidOperationException("Failed to arrange the function call. Use a debug build of libclang-pathogen for details."); }
        }

        public PathogenArrangedFunction(PathogenCodeGenerator codeGenerator, FunctionDecl functionDecl)
            : this(codeGenerator, functionDecl.Handle)
        { }

        public PathogenArrangedFunction(PathogenCodeGenerator codeGenerator, CXType type)
            => Handle = PathogenExtensions.pathogen_GetArrangedFunctionPointer(codeGenerator.Handle, type);

        public PathogenArrangedFunction(PathogenCodeGenerator codeGenerator, FunctionProtoType type)
            : this(codeGenerator, type.Handle)
        { }

        public readonly ref struct ArgumentAccessor
        {
            private readonly PathogenArgumentInfo* FirstArgument;
            public readonly int Count;

            internal ArgumentAccessor(RawArrangedFunction* handle, int argumentCount)
            {
                FirstArgument = (PathogenArgumentInfo*)(&handle[1]);
                Count = argumentCount;
            }

            public readonly ref PathogenArgumentInfo this[int i]
            {
                get
                {
                    if (i < 0 || i >= Count)
                    { throw new IndexOutOfRangeException(); }

                    return ref Unsafe.AsRef<PathogenArgumentInfo>(&FirstArgument[i]);
                }
            }

            public ArgumentEnumerator GetEnumerator()
                => new(FirstArgument, Count);
        }

        public ref struct ArgumentEnumerator
        {
            private readonly PathogenArgumentInfo* FirstArgument;
            private PathogenArgumentInfo* CurrentArgument;
            private readonly int Count;
            private int Index;

            internal ArgumentEnumerator(PathogenArgumentInfo* firstArgument, int count)
            {
                CurrentArgument = FirstArgument = firstArgument;
                Count = count;
                Index = 0;
            }

            public ref readonly PathogenArgumentInfo Current => ref Unsafe.AsRef<PathogenArgumentInfo>(CurrentArgument);

            public bool MoveNext()
            {
                if (Index >= Count)
                { return false; }

                CurrentArgument = &FirstArgument[Index];
                Index++;
                return true;
            }
        }

        public void Dispose()
        {
            if (Handle is null)
            { return; }

            GC.SuppressFinalize(this);
            PathogenExtensions.pathogen_DisposeArrangedFunction(Handle);
            Handle = null;
        }

        ~PathogenArrangedFunction()
            => Dispose();
    }
}
