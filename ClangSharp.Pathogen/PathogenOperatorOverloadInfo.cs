using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PathogenOperatorOverloadInfo
    {
        public readonly PathogenOperatorOverloadKind Kind;
        private readonly IntPtr _Name;
        private readonly IntPtr _Spelling;
        private readonly byte _IsUnary;
        private readonly byte _IsBinary;
        private readonly byte _IsMemberOnly;

        public string Name => Marshal.PtrToStringAnsi(_Name);
        public string Spelling => Marshal.PtrToStringAnsi(_Spelling);
        public bool IsUnary => _IsUnary != 0;
        public bool IsBinary => _IsBinary != 0;
        public bool IsMemberOnly => _IsMemberOnly != 0;
    }
}
