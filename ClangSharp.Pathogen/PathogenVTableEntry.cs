using ClangSharp.Interop;
using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PathogenVTableEntry
    {
        public PathogenVTableEntryKind Kind;

        /// <remarks>
        /// Only relevant when <see cref="Kind"/> is one of
        /// <see cref="PathogenVTableEntryKind.FunctionPointer"/>,
        /// <see cref="PathogenVTableEntryKind.CompleteDestructorPointer"/>,
        /// <see cref="PathogenVTableEntryKind.DeletingDestructorPointer"/>, or
        /// <see cref="PathogenVTableEntryKind.UnusedFunctionPointer"/>
        /// </remarks>
        public CXCursor MethodDeclaration;

        /// <remarks>Only relevant when <see cref="Kind"/> is <see cref="PathogenVTableEntryKind.RTTI"/></remarks>
        public CXCursor RttiType;

        /// <remarks>
        /// Only relevant when <see cref="Kind"/> is ony of
        /// <see cref="PathogenVTableEntryKind.VCallOffset"/>,
        /// <see cref="PathogenVTableEntryKind.VBaseOffset"/>, or
        /// <see cref="PathogenVTableEntryKind.OffsetToTop"/>
        /// </remarks>
        public long Offset;
    }
}
