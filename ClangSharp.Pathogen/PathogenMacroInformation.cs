using ClangSharp.Interop;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClangSharp.Pathogen
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct PathogenMacroInformation
    {
        private readonly byte* _Name;
        private readonly ulong _NameLength;
        public readonly CXSourceLocation Location;
        private readonly byte _WasUndefined;
        private readonly byte _IsFunctionLike;
        private readonly byte _IsBuiltinMacro;
        private readonly byte _HasCommaPasting;
        private readonly byte _IsUsedForHeaderGuard;
        public readonly PathogenMacroVardicKind VardicKind;
        public readonly int ParameterCount;
        private readonly byte** ParameterNames;
        private readonly ulong* ParameterNameLengths;
        public readonly int TokenCount;
        private readonly byte* _RawValueSourceString;
        private readonly ulong _RawValueSourceStringLength;

        public string Name => Encoding.UTF8.GetString(_Name, checked((int)_NameLength));
        public bool WasUndefined => _WasUndefined != 0;
        public bool IsFunctionLike => _IsFunctionLike != 0;
        public bool IsBuiltinMacro => _IsBuiltinMacro != 0;
        public bool HasCommaPasting => _HasCommaPasting != 0;
        public bool IsUsedForHeaderGuard => _IsUsedForHeaderGuard != 0;
        public string RawValueSourceString => Encoding.UTF8.GetString(_RawValueSourceString, checked((int)_RawValueSourceStringLength));

        public string GetParameterName(int index)
        {
            if (index < 0 || index >= ParameterCount)
            { throw new ArgumentOutOfRangeException(nameof(index)); }

            return Encoding.UTF8.GetString(ParameterNames[index], checked((int)ParameterNameLengths[index]));
        }
    }
}
