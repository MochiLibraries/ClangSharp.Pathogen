using System.Collections.Generic;

namespace ClangSharp.Pathogen.InfoDumperSupport
{
    internal class FunctionDeclDumper : ReflectionDumper
    {
        public FunctionDeclDumper()
            : base(typeof(FunctionDecl))
        { }

        public override IEnumerable<InfoRow> Dump(object target, ClangSharpInfoDumper.Options options)
        {
            foreach (InfoRow row in base.Dump(target, options))
            { yield return row; }

            FunctionDecl targetFunction = (FunctionDecl)target;
            PathogenOperatorOverloadInfo info = targetFunction.GetOperatorOverloadInfo();
            bool isOperatorOverload = info.Kind != PathogenOperatorOverloadKind.None;
            yield return new InfoRow("IsOperatorOverload", isOperatorOverload.ToString(), isOperatorOverload);

            if (isOperatorOverload)
            {
                yield return InfoRow.MinorHeader("Operator overload info");
                yield return new InfoRow(nameof(info.Kind), info.Kind);
                yield return new InfoRow(nameof(info.Name), info.Name);
                yield return new InfoRow(nameof(info.Spelling), info.Spelling);
                yield return new InfoRow(nameof(info.IsUnary), info.IsUnary);
                yield return new InfoRow(nameof(info.IsBinary), info.IsBinary);
                yield return new InfoRow(nameof(info.IsMemberOnly), info.IsMemberOnly);
            }
        }
    }
}
