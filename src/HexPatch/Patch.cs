namespace HexPatch
{
    public class Patch
    {
        public string Description {get;set;}
        public string Template {get;set;}
        public string Substitution {get;set;}
        public string Type {get;set;} = string.Empty;
        public MatchWindow? Window {get;set;} = null;
    }
}