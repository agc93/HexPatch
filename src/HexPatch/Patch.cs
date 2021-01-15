namespace HexPatch
{
    public class Patch
    {
        public string Description {get;set;}
        public string Template {get;set;}
        public string Substitution {get;set;}
        public SubstitutionType Type {get;set;} = SubstitutionType.Before;
    }
}