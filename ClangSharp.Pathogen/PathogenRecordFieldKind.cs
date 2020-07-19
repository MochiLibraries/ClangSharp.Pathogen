namespace ClangSharp.Pathogen
{
    public enum PathogenRecordFieldKind : int
    {
        Normal,
        VTablePtr,
        NonVirtualBase,
        /// <remarks>Only appears in Microsoft ABI</remarks>
        VirtualBaseTablePtr,
        /// <remarks>Only appears in Microsoft ABI</remarks>
        VTorDisp,
        VirtualBase,
    }
}
