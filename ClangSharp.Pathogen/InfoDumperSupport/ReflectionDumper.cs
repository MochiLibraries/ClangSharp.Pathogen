using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DotNetType = System.Type;

namespace ClangSharp.Pathogen.InfoDumperSupport
{
    internal class ReflectionDumper : Dumper
    {
        private readonly DotNetType Type;
        private readonly List<PropertyOrFieldInfo> DataMembers;

        public ReflectionDumper(DotNetType type)
        {
            Type = type;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.DoNotWrapExceptions;
            FieldInfo[] fields = type.GetFields(bindingFlags);
            PropertyInfo[] properties = type.GetProperties(bindingFlags);

            DataMembers = new List<PropertyOrFieldInfo>(capacity: fields.Length + properties.Length);

            foreach (FieldInfo field in fields)
            { DataMembers.Add(new PropertyOrFieldInfo(field)); }

            foreach (PropertyInfo property in properties)
            {
                if (!PropertyFilter(Type, property))
                { continue; }

                DataMembers.Add(new PropertyOrFieldInfo(property));
            }
        }

        private static bool PropertyFilter(DotNetType type, PropertyInfo property)
        {
            // Set-only properties aren't useful
            if (!property.CanRead)
            { return false; }

            // Collections aren't useful for printing
            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            { return false; }

            // If we got this far, the property is OK
            return true;
        }

        public override IEnumerable<InfoRow> Dump(object target, ClangSharpInfoDumper.Options options)
        {
            yield return InfoRow.MinorHeader(Type.Name);

            foreach (PropertyOrFieldInfo dataMember in DataMembers)
            {
                object rawValue;
                string value;

                try
                {
                    rawValue = dataMember.GetValue(target);
                    value = rawValue?.ToString();
                }
                catch (Exception ex)
                {
                    rawValue = ex;
                    value = $"{ex.GetType()}: {ex.Message}";
                }

                // If the value is a declaration, add location info
                // (Except for translation units because their ToString prints the file name already.)
                if (rawValue is Cursor cursor && !(cursor is TranslationUnitDecl))
                { value += $" @ {cursor.GetFriendlyLocation()}"; }

                yield return new InfoRow(dataMember.Name, value, rawValue);
            }
        }
    }
}
