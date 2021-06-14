namespace HexPatch
{
    public record ByteReplacement {
        public int MatchOffset {get;init;}
        public byte[] Key {get;init;}
        public byte[] Replacement {get; init;}
    }
}