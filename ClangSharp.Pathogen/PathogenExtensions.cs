using ClangSharp.Interop;
using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Pathogen
{
    public class PathogenExtensions
    {
        private const string LibraryFileName = "libclang-pathogen.dll";

        private struct PathogenTypeSizes
        {
            public int SizeOfPathogenTypeSizes;
            public int PathogenRecordLayout;
            public int PathogenRecordField;
            public int PathogenVTable;
            public int PathogenVTableEntry;
            public int PathogenOperatorOverloadInfo;
            public int PathogenConstantString;
            public int PathogenConstantValueInfo;
            public int PathogenMacroInformation;
        }

        [DllImport(LibraryFileName, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern byte pathogen_GetTypeSizes(ref PathogenTypeSizes sizes);

        unsafe static PathogenExtensions()
        {
            // Validate the size of types between C# and C++ as a sanity check
            PathogenTypeSizes sizes = new PathogenTypeSizes()
            {
                SizeOfPathogenTypeSizes = sizeof(PathogenTypeSizes)
            };

            if (pathogen_GetTypeSizes(ref sizes) == 0)
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenTypeSizes)} is wrong."); }

            if (sizes.PathogenRecordLayout != sizeof(PathogenRecordLayout))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenRecordLayout)} is wrong."); }

            if (sizes.PathogenRecordField != sizeof(PathogenRecordField))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenRecordField)} is wrong."); }

            if (sizes.PathogenVTable != sizeof(PathogenVTable))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenVTable)} is wrong."); }

            if (sizes.PathogenVTableEntry != sizeof(PathogenVTableEntry))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenVTableEntry)} is wrong."); }

            if (sizes.PathogenOperatorOverloadInfo != sizeof(PathogenOperatorOverloadInfo))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenOperatorOverloadInfo)} is wrong."); }

            if (sizes.PathogenConstantString != sizeof(PathogenConstantString))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenConstantString)} is wrong."); }

            if (sizes.PathogenConstantValueInfo != sizeof(PathogenConstantValueInfo))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenConstantValueInfo)} is wrong."); }

            if (sizes.PathogenMacroInformation != sizeof(PathogenMacroInformation))
            { throw new InvalidOperationException($"Cannot initialize Pathogen libclang extensions, sizeof({nameof(PathogenMacroInformation)} is wrong."); }
        }

        [DllImport(LibraryFileName, ExactSpelling = true)]
        public static extern unsafe PathogenRecordLayout* pathogen_GetRecordLayout(CXCursor cursor);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        public static extern unsafe void pathogen_DeleteRecordLayout(PathogenRecordLayout* layout);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool pathogen_Location_isFromMainFile(CXSourceLocation location);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        internal static extern unsafe PathogenOperatorOverloadInfo* pathogen_getOperatorOverloadInfo(CXCursor cursor);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        internal static extern PathogenArgPassingKind pathogen_getArgPassingRestrictions(CXCursor cursor);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern unsafe bool pathogen_ComputeConstantValue(CXCursor cursor, PathogenConstantValueInfo* info, byte** error);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        public static extern unsafe void pathogen_DeletePathogenConstantValueInfo(PathogenConstantValueInfo* info);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        public static extern uint pathogen_GetPreprocessorIdentifierCount(CXTranslationUnit translationUnit);

        [DllImport(LibraryFileName, ExactSpelling = true)]
        public static extern unsafe void pathogen_EnumerateMacros(CXTranslationUnit translationUnit, delegate* unmanaged[Cdecl]<PathogenMacroInformation*, void*, void> enumerator, void* userData);
    }
}
