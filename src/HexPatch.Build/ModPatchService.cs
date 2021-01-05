using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;

namespace HexPatch.Build {
    public class ModPatchService {
        private readonly FilePatcher _patcher;
        private readonly BuildContextFactory _factory;

        public ModPatchService(FilePatcher patcher, GameFileService fileService, BuildContextFactory contextFactory) {
            _patcher = patcher;
            _factory = contextFactory;
        }

        public async Task RunPatches(IEnumerable<KeyValuePair<string, Mod>> modCollection) {
            var mods = modCollection.ToList();
            var ctx = await _factory.Create(null);
            var requiredFiles = mods
                .SelectMany(em => em.Value.FilePatches)
                .GroupBy(fp => fp.Key)
                .Select(g => g.Key)
                .Distinct()
                .ToList();
            foreach (var file in requiredFiles) {
                ctx.
            }
        }
        
        private void LoadFiles
    }
}