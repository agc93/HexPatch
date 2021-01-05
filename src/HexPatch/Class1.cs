using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HexPatch
{
    public class Patch
    {
        public string Description {get;set;}
        public string Template {get;set;}
        public string Substitution {get;set;}
        public SubstitutionType Type {get;set;} = SubstitutionType.Before;
    }

    public enum SubstitutionType {
        InPlace,
        Before,
        After,
        Between
    }

    public class PatchSet {
        public string Name {get;set;}
        public List<Patch> Patches {get;set;}
    }

    public class FilePatch {
        public string FileName {get;set;}
        public PatchSet RequiredPatches {get;set;}
        public List<PatchSet> OptionalPatches {get;set;}
    }

    public class SetMetadata {
        public string DisplayName {get;set;}
        public string Author {get;set;}
        public string GameVersion {get;set;}
    }

    public class Mod {
        [JsonPropertyName("_meta")]
        public SetMetadata Metadata {get;set;}
        public Dictionary<string, List<PatchSet>> FilePatches {get;set;}
        
    }
}
