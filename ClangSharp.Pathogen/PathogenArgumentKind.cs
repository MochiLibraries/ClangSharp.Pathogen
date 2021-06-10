namespace ClangSharp.Pathogen
{
    public enum PathogenArgumentKind : byte
    {
        Direct,
        Extend,
        Indirect,
        Ignore,
        Expand,
        CoerceAndExpand,
        InAlloca
    }
}
