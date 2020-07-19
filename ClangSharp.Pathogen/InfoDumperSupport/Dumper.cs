using System.Collections.Generic;

namespace ClangSharp.Pathogen.InfoDumperSupport
{
    internal abstract class Dumper
    {
        public abstract IEnumerable<InfoRow> Dump(object target, ClangSharpInfoDumper.Options options);
    }
}
