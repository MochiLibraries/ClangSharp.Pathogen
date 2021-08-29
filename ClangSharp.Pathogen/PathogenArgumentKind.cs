namespace ClangSharp.Pathogen
{
    public enum PathogenArgumentKind : byte
    {
        Direct,
        Extend,
        Indirect,
        IndirectAliased,
        Ignore,
        Expand,
        CoerceAndExpand,
        InAlloca
    }
}
