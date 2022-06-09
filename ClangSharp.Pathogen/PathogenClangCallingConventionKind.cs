namespace ClangSharp.Pathogen
{
    public enum PathogenClangCallingConventionKind : byte
    {
        C,
        X86StdCall,
        X86FastCall,
        X86ThisCall,
        X86VectorCall,
        X86Pascal,
        Win64,
        X86_64SysV,
        X86RegCall,
        AAPCS,
        AAPCS_VFP,
        IntelOclBicc,
        SpirFunction,
        OpenCLKernel,
        Swift,
        SwiftAsync,
        PreserveMost,
        PreserveAll,
        AArch64VectorCall,
    }
}
