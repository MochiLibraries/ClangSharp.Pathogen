using ClangSharp.Interop;
using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PathogenRecordField
    {
        public PathogenRecordFieldKind Kind;
        public long Offset;
        public PathogenRecordField* NextField;
        public CXString Name;

        /// <remarks>
        /// This is the type of the field when <see cref="Kind"/> is <see cref="PathogenRecordFieldKind.Normal"/>.
        /// 
        /// This is the type of the base when <see cref="Kind"/> is one of
        /// <see cref="PathogenRecordFieldKind.NonVirtualBase"/>,
        /// <see cref="PathogenRecordFieldKind.VTorDisp"/>, or
        /// <see cref="PathogenRecordFieldKind.VirtualBase"/>.
        /// 
        /// This is <see cref="void*"/> when <see cref="Kind"/> is either <see cref="PathogenRecordFieldKind.VTablePtr"/> or <see cref="PathogenRecordFieldKind.VirtualBaseTablePtr"/>.
        /// </remarks>
        public CXType Type;

        /// <remarks>Only relevant when <see cref="Kind"/> is <see cref="PathogenRecordFieldKind.Normal"/></remarks>
        public CXCursor FieldDeclaration;
        public byte IsBitField;

        /// <remarks>Only relevant when <see cref="IsBitField"/> is true.</remarks>
        public uint BitFieldStart;
        /// <remarks>Only relevant when <see cref="IsBitField"/> is true.</remarks>
        public uint BitFieldWidth;

        /// <remarks>Only relevant when <see cref="Kind"/> is <see cref="PathogenRecordFieldKind.NonVirtualBase"/> or <see cref="PathogenRecordFieldKind.VirtualBase"/>.</remarks>
        public byte IsPrimaryBase;
    }
}
