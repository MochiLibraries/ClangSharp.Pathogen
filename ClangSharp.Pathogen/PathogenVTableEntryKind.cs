namespace ClangSharp.Pathogen
{
    public enum PathogenVTableEntryKind : int
    {
        VCallOffset,
        VBaseOffset,
        OffsetToTop,
        RTTI,
        FunctionPointer,
        CompleteDestructorPointer,
        DeletingDestructorPointer,
        UnusedFunctionPointer,
    }

    public static class PathogenVTableEntryKindEx
    {
        public static bool IsFunctionPointerKind(this PathogenVTableEntryKind kind)
        {
            switch (kind)
            {
                case PathogenVTableEntryKind.FunctionPointer:
                case PathogenVTableEntryKind.CompleteDestructorPointer:
                case PathogenVTableEntryKind.DeletingDestructorPointer:
                case PathogenVTableEntryKind.UnusedFunctionPointer:
                    return true;
                default:
                    return false;
            }
        }
    }
}
