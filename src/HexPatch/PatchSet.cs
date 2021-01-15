using System.Collections.Generic;

namespace HexPatch
{
    public class PatchSet {
        public string Name {get;set;}
        public List<Patch> Patches {get;set;}
    }
}