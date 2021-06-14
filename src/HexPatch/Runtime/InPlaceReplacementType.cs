using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HexPatch.Runtime
{
    public class InPlaceReplacementType : IReplacementType
    {
        public string Name => "InPlace";
        public byte[] ReplaceBytes(byte[] src, IEnumerable<ByteReplacement>? replacements)
        {
            var dst = new byte[src.Length + Convert.ToInt32(Math.Abs(src.Length*0.25))];
            var lastIndex = 0;
            replacements = (replacements ?? new List<ByteReplacement>()).OrderBy(r => r.MatchOffset);
            var srcStream = new MemoryStream(src);
            var tgtStream = new MemoryStream(dst);
            foreach (var replacement in replacements)
            {
                var tgt = new List<byte[]>();
                var unchanged = Convert.ToInt32(replacement.MatchOffset - srcStream.Position);
                // before found array
                srcStream.CopyToStream(tgtStream, unchanged);
                tgtStream.Write(replacement.Replacement);
                srcStream.Seek(replacement.Key.Length, SeekOrigin.Current);

                lastIndex = (int)srcStream.Position;
            }
            srcStream.CopyToStream(tgtStream, src.Length - lastIndex);
            var finalLength = tgtStream.Position;
            var final = new byte[finalLength];
            tgtStream.Seek(0, SeekOrigin.Begin);
            tgtStream.Read(final, 0, (int)finalLength);
            return final;
        }
    }
}