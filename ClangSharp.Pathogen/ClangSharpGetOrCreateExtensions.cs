using ClangSharp.Interop;
using System;
using System.Reflection;
using ClangType = ClangSharp.Type;
using DotNetType = System.Type;

namespace ClangSharp.Pathogen
{
    public static class ClangSharpGetOrCreateExtensions
    {
        private static MethodInfo TranslationUnit_GetOrCreate_CXCursor;
        private static MethodInfo TranslationUnit_GetOrCreate_CXType;
        [ThreadStatic] private static object[] TranslationUnit_GetOrCreate_Parameters;

        public static Cursor GetOrCreate(this TranslationUnit translationUnit, CXCursor handle)
        {
            if (handle.TranslationUnit != translationUnit.Handle)
            { throw new ArgumentException("The specified cursor is not from the specified translation unit.", nameof(handle)); }

            if (TranslationUnit_GetOrCreate_CXCursor == null)
            {
                DotNetType[] parameterTypes = { typeof(CXCursor) };
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DoNotWrapExceptions;
                MethodInfo getOrCreateGeneric = typeof(TranslationUnit).GetMethod("GetOrCreate", genericParameterCount: 1, bindingFlags, binder: null, parameterTypes, modifiers: null);

                if (getOrCreateGeneric is null)
                { throw new NotSupportedException("Could not get the GetOrCreate<TCursor>(CXCursor) method!"); }

                TranslationUnit_GetOrCreate_CXCursor = getOrCreateGeneric.MakeGenericMethod(typeof(Cursor));
            }

            if (TranslationUnit_GetOrCreate_Parameters == null)
            { TranslationUnit_GetOrCreate_Parameters = new object[1]; }

            TranslationUnit_GetOrCreate_Parameters[0] = handle; //PERF: Frequent boxing
            return (Cursor)TranslationUnit_GetOrCreate_CXCursor.Invoke(translationUnit, TranslationUnit_GetOrCreate_Parameters);
        }

        public static ClangType GetOrCreate(this TranslationUnit translationUnit, CXType handle)
        {
            // This has issues with built-in types. Unclear how important this check even is, so it's disabled for now.
            // In theory we could just check this when there is a declaration, but built-in types seem to have invalid declarations rather than just null ones.
            //if (handle.Declaration.TranslationUnit != translationUnit.Handle)
            //{ throw new ArgumentException("The specified type is not from the specified translation unit.", nameof(handle)); }

            if (TranslationUnit_GetOrCreate_CXType == null)
            {
                DotNetType[] parameterTypes = { typeof(CXType) };
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DoNotWrapExceptions;
                MethodInfo getOrCreateGeneric = typeof(TranslationUnit).GetMethod("GetOrCreate", genericParameterCount: 1, bindingFlags, binder: null, parameterTypes, modifiers: null);

                if (getOrCreateGeneric is null)
                { throw new NotSupportedException("Could not get the GetOrCreate<TType>(CXType) method!"); }

                TranslationUnit_GetOrCreate_CXType = getOrCreateGeneric.MakeGenericMethod(typeof(ClangType));
            }

            if (TranslationUnit_GetOrCreate_Parameters == null)
            { TranslationUnit_GetOrCreate_Parameters = new object[1]; }

            TranslationUnit_GetOrCreate_Parameters[0] = handle; //PERF: Frequent boxing
            return (ClangType)TranslationUnit_GetOrCreate_CXType.Invoke(translationUnit, TranslationUnit_GetOrCreate_Parameters);
        }
    }
}
