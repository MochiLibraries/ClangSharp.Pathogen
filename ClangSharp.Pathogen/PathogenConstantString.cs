using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PathogenConstantString
    {
        public ulong SizeBytes;
        public byte FirstByte;
    }
}
