using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModEngine.Core;

namespace HexPatch
{
    public class HexPatchEngine : IPatchEngine<Patch>
    {
        private readonly FilePatcher _filePatcher;
        private readonly ILogger<HexPatchEngine>? _logger;

        public HexPatchEngine(FilePatcher filePatcher, ILogger<HexPatchEngine>? logger) {
            _filePatcher = filePatcher;
            _logger = logger;
        }
        public async Task<IEnumerable<FileInfo>> RunPatch(string sourceKey, IEnumerable<PatchSet<Patch>> sets, string? targetName = null) {
            var modified = new List<FileInfo>();
            var oldSets = new List<FilePatchSet>();
            var origLength = new FileInfo(sourceKey).Length;
            foreach (var patchSet in sets) {
                var newPatches = new List<FilePatch>();
                foreach (var patch in patchSet.Patches) {
                    newPatches.Add(new FilePatch {Description = patch.Description, Template = patch.Template, Type = patch.Type, Substitution = patch.Value});
                }
                oldSets.Add(new FilePatchSet {Name = patchSet.Name, Patches = newPatches});
            }
            var fi = await _filePatcher.RunPatch(sourceKey, oldSets, targetName);
            modified.Add(fi);
            var newSize = fi.Length;
            if (fi.Extension == ".uexp" && newSize != origLength) {
                var uaFilePath = Path.ChangeExtension(fi.FullName, ".uasset");
                var uaFile = new FileInfo(uaFilePath);
                if (uaFile.Exists)
                {
                    _logger?.LogDebug("Detected matching uasset file, attempting to patch length");
                    var lengthBytes = BitConverter.ToString(BitConverter.GetBytes(((int) origLength - 4))).Replace("-", string.Empty);
                    var correctedBytes = BitConverter.ToString(BitConverter.GetBytes(((int) fi.Length - 4))).Replace("-", string.Empty);
                    var lPatch = new FilePatchSet()
                    {
                        Name = "Length auto-correct",
                        Patches = new List<FilePatch>
                        {
                            new FilePatch
                            {
                                Description = "uexp Length",
                                Template = lengthBytes,
                                Substitution = correctedBytes,
                                Type = "inPlace"
                            }
                        }
                    };
                    var finalFile = await _filePatcher.RunPatch(uaFile.FullName, new List<FilePatchSet>{lPatch});
                    modified.Add(finalFile);
                }
            }
            return modified;
        }
        
        public static class HexPatchHelpers
        {
            public static Dictionary<string, IEnumerable<PatchSet<Patch>>> ConvertPatches(
                Dictionary<string, List<FilePatchSet>> filePatches) {
                var tgt = new Dictionary<string, IEnumerable<PatchSet<Patch>>>();
                foreach (var (file, patchSets) in filePatches) {
                    var tgtPatchSets = new List<PatchSet<Patch>>();
                    foreach (var patchSet in patchSets) {
                        var newPatches = new List<Patch>();
                        foreach (var patch in patchSet.Patches) {
                            newPatches.Add(new Patch {Description = patch.Description, Template = patch.Template, Type = patch.Type, Value = patch.Substitution});
                        }
                        tgtPatchSets.Add(new PatchSet<Patch> {Name = patchSet.Name, Patches = newPatches});
                    }
                    tgt.Add(file, tgtPatchSets);
                }
                return tgt;
            }
        }
    }

    
}