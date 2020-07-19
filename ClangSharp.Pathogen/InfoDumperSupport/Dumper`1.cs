using System.Collections.Generic;

namespace ClangSharp.Pathogen.InfoDumperSupport
{
    internal abstract class Dumper<T> : Dumper
    {
        protected abstract IEnumerable<InfoRow> DumpT(T target, ClangSharpInfoDumper.Options options);

        public sealed override IEnumerable<InfoRow> Dump(object target, ClangSharpInfoDumper.Options options)
            => DumpT((T)target, options);
    }
}
