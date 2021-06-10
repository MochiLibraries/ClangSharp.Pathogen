using System;

namespace ClangSharp.Pathogen
{
    [Flags]
    public enum PathogenArgumentFlags : ushort
    {
        None = 0,
        /// <summary>Requires Kind = Direct, Extend, or CoerceAndExpand</summary>
        HasCoerceToTypeType = 1,
        /// <summary>Requires Kind = Direct, Extend, Indirect, or Expand</summary>
        HasPaddingType = 2,
        /// <summary>Requires Kind = CoerceAndExpand</summary>
        HasUnpaddedCoerceAndExpandType = 4,
        /// <summary>Applies to any kind</summary>
        PaddingInRegister = 8,
        /// <summary>Requires Kind = InAlloca</summary>
        IsInAllocaSRet = 16,
        /// <summary>Requires Kind = Indirect</summary>
        IsIndirectByVal = 32,
        /// <summary>Requires Kind = Indirect</summary>
        IsIndirectRealign = 64,
        /// <summary>Requires Kind = Indirect</summary>
        IsSRetAfterThis = 128,
        /// <summary>Requires Kind = Direct, Extend, or Indirect</summary>
        IsInRegister = 256,
        /// <summary>Requires Kind = Direct</summary>
        CanBeFlattened = 512,
        /// <summary>Requires Kind = Extend</summary>
        IsSignExtended = 1024
    }
}
