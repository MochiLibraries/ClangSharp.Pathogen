using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PathogenRecordLayout
    {
        public PathogenRecordField* FirstField;
        public PathogenVTable* FirstVTable;

        public long Size;
        public long Alignment;

        /// <remarks>For C++ records only.</remarks>
        public byte IsCppRecord;
        /// <remarks>For C++ records only.</remarks>
        public long NonVirtualSize;
        /// <remarks>For C++ records only.</remarks>
        public long NonVirtualAlignment;
    }
}
