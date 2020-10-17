namespace ClangSharp.Pathogen
{
    /// <remarks>
    /// This enum corresponds to <c>APValue::ValueKind</c>
    /// 
    /// Unlike most Clang enums, this one is not duplicated+verified in <c>PathogenExtensions.cpp</c>. As such, this enum is primarily intended for diagnostic use only.
    /// </remarks>
    public enum ClangConstantValueKind : int
    {
        None,
        Indeterminate,
        Int,
        Float,
        FixedPoint,
        ComplexInt,
        ComplexFloat,
        LValue,
        Vector,
        Array,
        Struct,
        Union,
        MemberPointer,
        AddrLabelDiff
    }
}
