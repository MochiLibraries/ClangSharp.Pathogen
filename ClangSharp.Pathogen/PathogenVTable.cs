using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PathogenVTable
    {
        public int EntryCount;
        public PathogenVTableEntry* Entries;

        public PathogenVTable* NextVTable;
    }
}
