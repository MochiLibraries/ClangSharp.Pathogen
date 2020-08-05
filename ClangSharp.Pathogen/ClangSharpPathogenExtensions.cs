using ClangSharp.Interop;
using System;
using System.Runtime.CompilerServices;

namespace ClangSharp.Pathogen
{
    public static class ClangSharpPathogenExtensions
    {
        public static bool IsFromMainFilePathogen(this CXSourceLocation location)
            => PathogenExtensions.pathogen_Location_isFromMainFile(location);

        public static unsafe ref PathogenOperatorOverloadInfo GetOperatorOverloadInfo(this CXCursor cursor)
        {
            if (cursor.DeclKind < CX_DeclKind.CX_DeclKind_FirstFunction || cursor.DeclKind > CX_DeclKind.CX_DeclKind_LastFunction)
            { throw new ArgumentException("The specified cursor must be a function declaration of some kind.", nameof(cursor)); }

            PathogenOperatorOverloadInfo* ret = PathogenExtensions.pathogen_getOperatorOverloadInfo(cursor);

            // If null was returned there's something wrong with this function
            if (ret == null)
            { throw new ArgumentException("The specified declaration is not actually a function.", nameof(cursor)); }

            // If the returned structure has an invalid operator overload kind, the function is an operator overload but Clang returned an unexpected value
            if (ret->Kind == PathogenOperatorOverloadKind.Invalid)
            { throw new NotSupportedException("The specified function is an operator overload, but Clang returned an unexpected operator overload kind."); }

            return ref Unsafe.AsRef<PathogenOperatorOverloadInfo>(ret);
        }

        public static ref PathogenOperatorOverloadInfo GetOperatorOverloadInfo(this FunctionDecl function)
            => ref function.Handle.GetOperatorOverloadInfo();

        public static PathogenArgPassingKind GetRecordArgPassingRestrictions(this CXCursor cursor)
            => PathogenExtensions.pathogen_getArgPassingRestrictions(cursor);

        public static PathogenArgPassingKind GetArgPassingRestrictions(this RecordDecl record)
            => record.Handle.GetRecordArgPassingRestrictions();
    }
}
