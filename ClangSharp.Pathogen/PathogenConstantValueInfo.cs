using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PathogenConstantValueInfo
    {
        public bool HasSideEffects;
        public bool HasUndefinedBehavior;
        public PathogenConstantValueKind Kind;
        public int SubKind;
        public ulong Value;
    }
}
