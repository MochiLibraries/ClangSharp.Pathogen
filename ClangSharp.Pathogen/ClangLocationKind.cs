namespace ClangSharp.Pathogen
{
    /// <remarks>Instantiation location is not included in this enum because it is deprecated, it was replaced by <see cref="ExpansionLocation"/>.</remarks>
    public enum ClangLocationKind
    {
        Spelling,
        ExpansionLocation,
        FileLocation,
        PresumedLocation
    }
}
