using ClangSharp.Interop;

namespace ClangSharp.Pathogen
{
    public readonly struct PathogenArgumentInfo
    {
        public readonly CXType Type;
        public readonly PathogenArgumentKind Kind;

        public readonly PathogenArgumentFlags Flags;

        /// <summary>
        /// For Kind = Direct or Extend: DirectOffset
        /// For Kind = Inidrect: IndirectAlignment
        /// For Kind = InAlloca: AllocaFieldIndex
        /// </summary>
        public readonly uint Extra;
    }
}
