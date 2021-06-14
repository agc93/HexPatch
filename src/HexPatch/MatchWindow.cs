namespace HexPatch
{
    public record MatchWindow {
        //[System.Text.Json.Serialization.JsonInclude]
        public string Before {get; set;}
        //[System.Text.Json.Serialization.JsonInclude]
        public string After {get;set;}
        public int? MaxMatches { get; init; } = null;
    }
}