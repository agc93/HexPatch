namespace HexPatch
{
    public class Patch
    {
        public string Description {get;set;}
        public string Template {get;set;}
        public string Substitution {get;set;}
        public SubstitutionType Type {get;set;} = SubstitutionType.None;
        public TemplateWindow? Window {get;set;} = null;
    }

    public record TemplateWindow {
        //[System.Text.Json.Serialization.JsonInclude]
        public string Before {get; init;}
        //[System.Text.Json.Serialization.JsonInclude]
        public string After {get;init;}
        public int? MaxMatches { get; init; } = null;
    }
}