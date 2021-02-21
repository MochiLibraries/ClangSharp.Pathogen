using ClangSharp.Interop;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClangSharp.Pathogen
{
    public static class ClangSharpExtensions
    {
        public static string CursorKindDetailed(this Cursor cursor, string delimiter = "/")
        {
            string kind = cursor.CursorKind.ToString();
            string declKind = cursor.Handle.DeclKind.ToString();

            const string kindPrefix = "CXCursor_";
            if (kind.StartsWith(kindPrefix))
            { kind = kind.Substring(kindPrefix.Length); }

            const string declKindPrefix = "CX_DeclKind_";
            if (declKind.StartsWith(declKindPrefix))
            { declKind = declKind.Substring(declKindPrefix.Length); }

            if (cursor.CursorKind == CXCursorKind.CXCursor_UnexposedDecl)
            { kind = null; }

            if (cursor.Handle.DeclKind == CX_DeclKind.CX_DeclKind_Invalid)
            { declKind = null; }

            string ret = cursor.GetType().Name;

            if (kind is object)
            { ret += $"{delimiter}{kind}"; }

            if (declKind is object)
            { ret += $"{delimiter}{declKind}"; }

            return ret;
        }

        public static CXCallingConv GetCallingConvention(this FunctionDecl function)
        {
            // When the convention is explicitly specified (with or without the __attribute__ syntax), function.Type will be AttributedType
            // Calling conventions that don't affect the current platform (IE: stdcall on x64) are ignored by Clang (they become CXCallingConv_C)
            if (function.Type is AttributedType attributedType)
            { return attributedType.Handle.FunctionTypeCallingConv; }
            else if (function.Type is FunctionType functionType)
            { return functionType.CallConv; }
            else
            { throw new NotSupportedException($"The function has an unexpected value for `{nameof(function.Type)}`."); }
        }

        public static CallingConvention ToDotNetCallingConvention(this CXCallingConv clangCallingConvention, out string errorMessage)
        {
            errorMessage = null;

            // https://github.com/llvm/llvm-project/blob/91801a7c34d08931498304d93fd718aeeff2cbc7/clang/include/clang/Basic/Specifiers.h#L269-L289
            // https://clang.llvm.org/docs/AttributeReference.html#calling-conventions
            // We generally expect this to always be cdecl on x64. (Clang supports some special calling conventions on x64, but C# doesn't support them.)
            switch (clangCallingConvention)
            {
                case CXCallingConv.CXCallingConv_C:
                    return CallingConvention.Cdecl;
                case CXCallingConv.CXCallingConv_X86StdCall:
                    return CallingConvention.StdCall;
                case CXCallingConv.CXCallingConv_X86FastCall:
                    return CallingConvention.FastCall;
                case CXCallingConv.CXCallingConv_X86ThisCall:
                    return CallingConvention.ThisCall;
                case CXCallingConv.CXCallingConv_Win64:
                    return CallingConvention.Winapi;
                case CXCallingConv.CXCallingConv_Invalid:
                    errorMessage = "Could not determine function's calling convention.";
                    return default;
                default:
                    errorMessage = $"Function uses unsupported calling convention '{clangCallingConvention}'.";
                    return default;
            }
        }

        public static unsafe CXFile GetFileLocation(this CXSourceLocation sourceLocation)
        {
            CXFile ret;
            clang.getFileLocation(sourceLocation, (void**)&ret, null, null, null);
            return ret;
        }

        public static PathogenTemplateSpecializationKind GetSpecializationKind(this ClassTemplateSpecializationDecl classTemplateSpecialization)
            => PathogenExtensions.pathogen_GetSpecializationKind(classTemplateSpecialization.Handle);

        public static unsafe string GetSpellingWithPlaceholder(this CXType type, string placeholder)
        {
            byte[] placeholderBytes = Encoding.UTF8.GetBytes(placeholder);
            fixed (byte* placeholderBytesP = placeholderBytes)
            {
                using CXString result = PathogenExtensions.pathogen_getTypeSpellingWithPlaceholder(type, placeholderBytesP, placeholderBytes.Length);
                return result.ToString();
            }
        }

        public static string GetSpellingWithPlaceholder(this Type type, string placeholder)
            => GetSpellingWithPlaceholder(type.Handle, placeholder);
    }
}
