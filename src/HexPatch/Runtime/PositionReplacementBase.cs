using System;
using System.Collections.Generic;
using System.Linq;

namespace HexPatch.Runtime
{
    public abstract class PositionReplacementBase : IReplacementType
    {
        protected byte[] ReplaceBytesBefore(byte[] src, IEnumerable<KeyValuePair<int, byte[]>> replacements, bool requireValue = false)
        {
            var dst = new byte[src.Length];
            var lastIndex = 0;
            var nextByte = 0;
            replacements = replacements.OrderBy(r => r.Key);
            foreach (var (key, replacement) in replacements)
            {
                // var changeOffset = repl.Key - lastIndex - repl.Value.Length;
                var unchanged = key - nextByte;
                // before found array
                Buffer.BlockCopy(src, nextByte, dst, nextByte, unchanged);
                if (requireValue)
                {
                    var buffer = new byte[replacement.Length];
                    Buffer.BlockCopy(src, key - replacement.Length, buffer, 0, replacement.Length);
                    if (buffer.All(b => b == 0x0))
                    {
                        Buffer.BlockCopy(src, key - replacement.Length, dst, key - replacement.Length, replacement.Length);
                    }
                    else
                    {
                        Buffer.BlockCopy(replacement, 0, dst, key - replacement.Length, replacement.Length);
                    }
                }
                else
                {
                    Buffer.BlockCopy(replacement, 0, dst, key - replacement.Length, replacement.Length);
                }
                // repl copy
                lastIndex = key;
                nextByte = key;
            }
            Buffer.BlockCopy(
                src,
                lastIndex,
                dst,
                lastIndex,
                src.Length - lastIndex);
            return dst;
        }

        public abstract string Name { get; }
        public abstract byte[] ReplaceBytes(byte[] srcBytes, IEnumerable<ByteReplacement>? replacements);
    }
}