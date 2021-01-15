using System;
using System.Collections.Generic;

namespace HexPatch
{
    public class FilePatch {
        public string FileName {get;set;}
        public PatchSet RequiredPatches {get;set;}
        public List<PatchSet> OptionalPatches {get;set;}
    }
}
