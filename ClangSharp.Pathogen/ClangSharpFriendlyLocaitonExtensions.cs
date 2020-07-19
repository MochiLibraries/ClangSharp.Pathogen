using ClangSharp.Interop;
using System;
using System.IO;

namespace ClangSharp.Pathogen
{
    public static class ClangSharpFriendlyLocaitonExtensions
    {
        public static string GetFriendlyLocation(this Cursor cursor, ClangLocationKind kind = ClangLocationKind.Spelling, bool includeFileKindInfo = false)
            => cursor.Extent.GetFriendlyLocation(kind, includeFileKindInfo);

        public static string GetFriendlyLocation(this CXSourceRange extent, ClangLocationKind kind = ClangLocationKind.Spelling, bool includeFileKindInfo = false)
        {
            CXFile startFile;
            uint startLine;
            uint startColumn;
            uint startOffset;
            CXFile endFile;
            uint endLine;
            uint endColumn;
            uint endOffset;
            CXString startFileName = default;
            CXString endFileName = default;

            switch (kind)
            {
                case ClangLocationKind.Spelling:
                    extent.Start.GetSpellingLocation(out startFile, out startLine, out startColumn, out startOffset);
                    extent.End.GetSpellingLocation(out endFile, out endLine, out endColumn, out endOffset);
                    break;
                case ClangLocationKind.ExpansionLocation:
                    extent.Start.GetExpansionLocation(out startFile, out startLine, out startColumn, out startOffset);
                    extent.End.GetExpansionLocation(out endFile, out endLine, out endColumn, out endOffset);
                    break;
                case ClangLocationKind.FileLocation:
                    extent.Start.GetFileLocation(out startFile, out startLine, out startColumn, out startOffset);
                    extent.End.GetFileLocation(out endFile, out endLine, out endColumn, out endOffset);
                    break;
                case ClangLocationKind.PresumedLocation:
                    extent.Start.GetPresumedLocation(out startFileName, out startLine, out startColumn);
                    extent.End.GetPresumedLocation(out endFileName, out endLine, out endColumn);
                    startFile = endFile = default;
                    startOffset = endOffset = default;
                    break;
                default:
                    throw new ArgumentException("The specified location kind is invalid.", nameof(kind));
            }

            string ret;

            if (kind != ClangLocationKind.PresumedLocation)
            { ret = FormatLocation(startFile, startLine, startColumn, startOffset, endFile, endLine, endColumn, endOffset); }
            else
            { ret = FormatLocation(startFileName.ToString(), startLine, startColumn, 0, endFileName.ToString(), endLine, endColumn, 0); }

            if (includeFileKindInfo)
            {
                if (extent.Start.IsFromMainFilePathogen() || extent.End.IsFromMainFilePathogen())
                { ret += " <MainFilePgn>"; }

                if (extent.Start.IsInSystemHeader || extent.End.IsInSystemHeader)
                { ret += " <SystemHeader>"; }
            }

            return ret;
        }

        private static string FormatLocation(CXFile startFile, uint startLine, uint startColumn, uint startOffset, CXFile endFile, uint endLine, uint endColumn, uint endOffset)
                => FormatLocation(startFile.Name.ToString(), startLine, startColumn, startOffset, endFile.Name.ToString(), endLine, endColumn, endOffset);

        private static string FormatLocation(string startFileName, uint startLine, uint startColumn, uint startOffset, string endFileName, uint endLine, uint endColumn, uint endOffset)
        {
            startFileName = Path.GetFileName(startFileName);
            endFileName = Path.GetFileName(endFileName);

            string location = "";

            if (startFileName == endFileName)
            {
                location += startFileName;
                location += startLine == endLine ? $":{startLine}" : $":{startLine}..{endLine}";
                location += startColumn == endColumn ? $":{startColumn}" : $":{startColumn}..{endColumn}";

                if (startOffset != 0 && endOffset != 0)
                { location += startOffset == endOffset ? $"[{startOffset}]" : $"[{startOffset}..{endOffset}]"; }
            }
            else
            { location += $" {startFileName}:{startLine}:{startColumn}[{startOffset}]..{endFileName}:{endLine}:{endColumn}[{endOffset}]"; }

            return location;
        }
    }
}
