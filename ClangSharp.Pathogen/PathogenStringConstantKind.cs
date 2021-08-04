using System;

namespace ClangSharp.Pathogen
{
    public enum PathogenStringConstantKind : int
    {
        Ascii,
        /// <summary>Never actually used. We replace this with the more appropriate UTF equivalent with WideCharBit set instead.</summary>
        [Obsolete("This kind will not appear under normal circumstances. It will be replaced with the appropriate UTF kind.")]
        WideChar,
        Utf8,
        Utf16,
        Utf32,
        /// <summary>When combined with one of the UTF values, indicates that the constant was originally a <c>wchar_t</c> string.</summary>
        WideCharBit = 1 << 31
    }
}
