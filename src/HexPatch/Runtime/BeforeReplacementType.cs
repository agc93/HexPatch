using System.Collections.Generic;
using System.Linq;

namespace HexPatch.Runtime
{
    public class BeforeReplacementType : PositionReplacementBase
    {
        public override string Name => "Before";
        public override byte[] ReplaceBytes(byte[] srcBytes, IEnumerable<ByteReplacement>? replacements) {
            replacements ??= new List<ByteReplacement>();
            return ReplaceBytesBefore(srcBytes,
                replacements.Select(r => new KeyValuePair<int, byte[]>(r.MatchOffset, r.Replacement)));
        }
    }
}