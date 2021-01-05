using System.Collections.Generic;
using System.IO;

namespace HexPatch.Build
{
    public class PatchBuildOptions {
        public List<string> FileSources { get; set; } = new List<string> {
            Path.Join(System.Environment.CurrentDirectory, "GameFiles")
        };
        public bool RecursiveFileSearch { get; set; } = false;
        public string OutputPath { get; set; } = System.Environment.CurrentDirectory;
    }
}