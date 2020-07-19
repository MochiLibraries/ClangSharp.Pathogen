using System.IO;

namespace ClangSharp.Pathogen.InfoDumperSupport
{
    internal readonly struct InfoRow
    {
        public readonly bool IsMajorHeader;
        public readonly bool IsMinorHeader;
        public readonly string Label;
        public readonly string Value;
        public readonly object RawValue;

        public InfoRow(string label, string value, object rawValue)
        {
            IsMajorHeader = false;
            IsMinorHeader = false;
            Label = label;
            RawValue = rawValue;

            if (value is null)
            { value = "<null>"; }
            else if (value.Length == 0)
            { value = "<empty>"; }
            // Bit of a hack, try to make paths prettier
            else if (Path.IsPathFullyQualified(value))
            { value = Path.GetFileName(value); }

            Value = value;
        }

        public InfoRow(string label, object rawValue)
            : this(label, rawValue?.ToString(), rawValue)
        { }

        private InfoRow(string header, bool isMajorHeader = false, bool isMinorHeader = false)
        {
            IsMajorHeader = isMajorHeader;
            IsMinorHeader = isMinorHeader;
            Label = header;
            Value = null;
            RawValue = null;
        }

        public static InfoRow MajorHeader(string header)
            => new InfoRow(header, true, false);

        public static InfoRow MinorHeader(string header)
            => new InfoRow(header, false, true);
    }
}
