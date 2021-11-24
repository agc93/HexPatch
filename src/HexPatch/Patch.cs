using ModEngine.Core;

namespace HexPatch
{
    public class FilePatch : Patch
    {
        // public string Description {get;set;}
        // public string Template {get;set;}

        public string Substitution
        {
            get => Value;
            set => Value = value;
        }

        // public string Type {get;set;} = string.Empty;
        // public MatchWindow? Window {get;set;} = null;
    }
}