using System.Collections.Generic;

namespace ClangSharp.Pathogen.InfoDumperSupport
{
    internal class CursorDumper : Dumper<Cursor>
    {
        protected override IEnumerable<InfoRow> DumpT(Cursor target, ClangSharpInfoDumper.Options options)
        {
            bool includeDetailedLocationInfo = (options & ClangSharpInfoDumper.Options.IncludeDetailedLocationInfo) != 0;
            string locationLabel = "Location";

            if (includeDetailedLocationInfo)
            { locationLabel = "Spelling Location"; }

            yield return new InfoRow(locationLabel, target.GetFriendlyLocation(includeFileKindInfo: true), null);

            if (includeDetailedLocationInfo)
            {
                yield return new InfoRow("Expansion Location", target.GetFriendlyLocation(ClangLocationKind.ExpansionLocation), null);
                yield return new InfoRow("File Location", target.GetFriendlyLocation(ClangLocationKind.FileLocation), null);
                yield return new InfoRow("Presumed Location", target.GetFriendlyLocation(ClangLocationKind.PresumedLocation), null);
            }
        }
    }
}
