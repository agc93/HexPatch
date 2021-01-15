using System.Collections.Generic;
using System.IO;

namespace HexPatch.Build
{
    public class SourceFileOptions {
        public List<string> FileSources { get; set; } = new List<string> {
            Path.Join(System.Environment.CurrentDirectory, "SourceFiles")
        };
        public bool RecursiveFileSearch { get; set; } = false;
    }
}