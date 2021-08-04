using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PathogenConstantValueInfo
    {
        public bool HasSideEffects;
        public bool HasUndefinedBehavior;
        public PathogenConstantValueKind Kind;
        /// <summary>The subkind of the constant, see remarks for details.</summary>
        /// <remarks>
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.UnsignedInteger"/>, <see cref="PathogenConstantValueKind.SignedInteger"/>, or <see cref="PathogenConstantValueKind.FloatingPoint"/>: This is the size of the value in bits.
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.String"/>, this is one of <see cref="PathogenStringConstantKind"/>, potentially with WideCharBit set in the case of wchar_t.
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.Unknown"/>, this is one of <see cref="ClangConstantValueKind"/>.
        /// </remarks>
        public int SubKind;
        /// <summary>The value of the constant, see remarks for details.</summary>
        /// <remarks>
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.NullPointer"/> or <see cref="PathogenConstantValueKind.Unknown"/>, this is 0.
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.UnsignedInteger"/> this is a zero-extended integer.
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.SignedInteger"/> this is a sign-extended integer.
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.FloatingPoint"/> this is the floating point value as bits and any unused bits are 0.
        /// If <see cref="Kind"/> is <see cref="PathogenConstantValueKind.String"/> this is a pointer to a <see cref="PathogenConstantString"/> representing the string.
        /// </remarks>
        public ulong Value;
    }
}
