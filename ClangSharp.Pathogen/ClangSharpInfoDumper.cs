using ClangSharp.Pathogen.InfoDumperSupport;
using System;
using System.Collections.Generic;
using System.IO;
using ClangType = ClangSharp.Type;
using DotNetType = System.Type;

namespace ClangSharp.Pathogen
{
    public class ClangSharpInfoDumper
    {
        [Flags]
        public enum Options
        {
            None = 0,
            IncludeDetailedLocationInfo = 1,
            IncludeTypeDetails = 2,
            IncludeRecursiveTypeDetails = 4
        }

        private static readonly Dictionary<DotNetType, Dumper> Dumpers = new Dictionary<DotNetType, Dumper>()
        {
            { typeof(Cursor), new CursorDumper() },
            { typeof(FunctionDecl), new FunctionDeclDumper() },
            { typeof(RecordDecl), new RecordDeclDumper() },
        };

        private static void EnumerateRows(List<InfoRow> rows, DotNetType type, DotNetType endType, object target, Options options)
        {
            // Enumerate the rows from the parent type first (unless this is the cursor type)
            if (type != endType)
            { EnumerateRows(rows, type.BaseType, endType, target, options); }

            // Check if we have a dumper for this type
            Dumper dumper;
            if (!Dumpers.TryGetValue(type, out dumper))
            {
                dumper = new ReflectionDumper(type);
                Dumpers.Add(type, dumper);
            }

            // Enumerate the rows
            rows.AddRange(dumper.Dump(target, options));
        }

        private static void AddTypeInfo(List<InfoRow> rows, string label, ClangType clangType, Options options)
        {
            rows.Add(InfoRow.MajorHeader($"{label} -- {clangType.Kind}/{clangType.GetType().Name} -- {clangType}"));
            EnumerateRows(rows, clangType.GetType(), typeof(ClangType), clangType, options);

            // For tag types, enumerate the declaration
            if (clangType is TagType tagType && tagType.Decl is object)
            {
                rows.Add(InfoRow.MajorHeader($"{label}.Decl -- {tagType.Decl.CursorKindDetailed()} -- {tagType.Decl}"));
                EnumerateRows(rows, tagType.Decl.GetType(), typeof(Cursor), tagType.Decl, options);
            }

            // Add type info related types (like the type pointed to by a pointer.)
            if ((options & Options.IncludeRecursiveTypeDetails) != 0)
            {
                // Function types are a special case
                if (clangType is FunctionType functionType)
                {
                    AddTypeInfo(rows, $"{label}.{nameof(functionType.ReturnType)}", functionType.ReturnType, options);

                    if (functionType is FunctionProtoType functionProtoType)
                    {
                        int i = 0;
                        foreach (ClangType parameterType in functionProtoType.ParamTypes)
                        {
                            AddTypeInfo(rows, $"{label}.{nameof(functionProtoType.ParamTypes)}[{i}]", parameterType, options);
                            i++;
                        }
                    }
                }

                (ClangType nestedType, string subLabel) = clangType switch
                {
                    PointerType pointerType => (pointerType.PointeeType, nameof(pointerType.PointeeType)),
                    ReferenceType referenceType => (referenceType.PointeeType, nameof(referenceType.PointeeType)),
                    ArrayType arrayType => (arrayType.ElementType, nameof(arrayType.ElementType)),
                    AttributedType attributedType => (attributedType.ModifiedType, nameof(attributedType.ModifiedType)),
                    ElaboratedType elaboratedType => (elaboratedType.NamedType, nameof(elaboratedType.NamedType)),
                    TypedefType typedefType => (typedefType.CanonicalType, nameof(typedefType.CanonicalType)),
                    _ => default
                };

                if (nestedType is object)
                {
                    AddTypeInfo(rows, $"{label}.{subLabel}", nestedType, options);
                }
            }
        }

        public static Options DefaultOptions { get; set; } = Options.None;

        public static void Dump(TextWriter writer, Cursor cursor)
            => Dump(writer, cursor, DefaultOptions);

        public static void Dump(TextWriter writer, Cursor cursor, Options options)
        {
            List<InfoRow> rows = new List<InfoRow>();

            // Add the cursor header row
            rows.Add(InfoRow.MajorHeader($"{cursor.CursorKindDetailed()} {cursor}"));

            // Add cursor rows
            EnumerateRows(rows, cursor.GetType(), typeof(Cursor), cursor, options);

            // Find all type fields so we can dump them too
            if ((options & Options.IncludeTypeDetails) != 0)
            {
                int endOfCursorInfo = rows.Count;
                for (int i = 0; i < endOfCursorInfo; i++)
                {
                    InfoRow row = rows[i];

                    if (row.RawValue is ClangType clangType)
                    { AddTypeInfo(rows, row.Label, clangType, options); }
                }
            }

            // Special cases for functions, add parameter info too
            if (cursor is FunctionDecl functionDeclaration)
            {
                int parameterNumber = 0;
                foreach (ParmVarDecl parameter in functionDeclaration.Parameters)
                {
                    rows.Add(InfoRow.MajorHeader($"Parameters[{parameterNumber}] -- {parameter.CursorKindDetailed()} {parameter}"));
                    EnumerateRows(rows, parameter.GetType(), typeof(Cursor), parameter, options);
                    parameterNumber++;
                }
            }

            // Determine the longest key and value for formatting purposes
            int longestLabel = 0;
            int longestValue = 0;
            foreach (InfoRow row in rows)
            {
                if (row.Label.Length > longestLabel)
                { longestLabel = row.Label.Length; }

                if (row.Value is object && row.Value.Length > longestValue)
                { longestValue = row.Value.Length; }
            }

            // Write out the information dump
            const string columnSeparator = " | ";
            int headerBorderLength = longestLabel + longestValue + columnSeparator.Length;
            string headerBorder = new String('=', headerBorderLength);

            foreach (InfoRow row in rows)
            {
                // Handle major heading rows
                if (row.IsMajorHeader)
                {
                    writer.WriteLine(headerBorder);
                    writer.WriteLine(row.Label);
                    writer.WriteLine(headerBorder);
                    continue;
                }

                // Handle minor heading rows
                if (row.IsMinorHeader)
                {
                    const string headingRowPrefix = "---- ";
                    writer.Write(headingRowPrefix);
                    writer.Write(row.Label);

                    for (int i = headingRowPrefix.Length + row.Label.Length; i < headerBorderLength; i++)
                    { writer.Write('-'); }

                    writer.WriteLine();
                    continue;
                }

                // Write normal rows
                writer.Write(row.Label);

                for (int i = row.Label.Length; i < longestLabel; i++)
                { writer.Write(' '); }

                writer.Write(columnSeparator);
                writer.WriteLine(row.Value);
            }
        }
    }
}
