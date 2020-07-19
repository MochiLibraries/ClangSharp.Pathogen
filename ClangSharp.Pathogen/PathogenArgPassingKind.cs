namespace ClangSharp.Pathogen
{
    public enum PathogenArgPassingKind : int
    {
        CanPassInRegisters,
        CannotPassInRegisters,
        CanNeverPassInRegisters,
        Invalid
    }
}
