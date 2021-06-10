using System;

namespace ClangSharp.Pathogen
{
    [Flags]
    public enum PathogenArrangedFunctionFlags : ushort
    {
        None = 0,
        IsInstanceMethod = 1,
        IsChainCall = 2,
        IsNoReturn = 4,
        IsReturnsRetained = 8,
        IsNoCallerSavedRegs = 16,
        HasRegParm = 32,
        IsNoCfCheck = 64,
        IsVariadic = 128,
        UsesInAlloca = 256,
        HasExtendedParameterInfo = 512,
    }
}
