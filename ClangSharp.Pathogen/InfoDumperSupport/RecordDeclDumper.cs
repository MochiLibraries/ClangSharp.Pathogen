using System.Collections.Generic;

namespace ClangSharp.Pathogen.InfoDumperSupport
{
    internal class RecordDeclDumper : ReflectionDumper
    {
        public RecordDeclDumper()
            : base(typeof(RecordDecl))
        { }

        public override IEnumerable<InfoRow> Dump(object target, ClangSharpInfoDumper.Options options)
        {
            foreach (InfoRow row in base.Dump(target, options))
            { yield return row; }

            RecordDecl targetRecord = (RecordDecl)target;
            PathogenArgPassingKind argPassingRestrictions = targetRecord.GetArgPassingRestrictions();
            yield return new InfoRow("ArgPassingRestructions", argPassingRestrictions.ToString(), argPassingRestrictions);
        }
    }
}
