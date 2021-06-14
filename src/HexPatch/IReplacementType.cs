using System.Collections.Generic;

namespace HexPatch
{
    public interface IReplacementType
    {
        public string Name { get; }
        public byte[] ReplaceBytes(byte[] srcBytes, IEnumerable<ByteReplacement>? replacements);
    }
}