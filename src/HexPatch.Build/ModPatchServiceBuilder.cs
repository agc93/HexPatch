using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using Microsoft.Extensions.Logging;

namespace HexPatch.Build
{
    /// <summary>
    /// This builder exists only as a very shitty wrapper over the ModPatchService to make it more DI-friendly.
    /// Inject a *Builder then use that to create however many services you need.
    /// It's shit. I know.
    /// </summary>
    public class ModPatchServiceBuilder
    {
        private readonly SourceFileService _fileService;
        private readonly FilePatcher _filePatcher;
        private readonly BuildContextFactory _ctxFactory;
        private readonly ILogger<ModPatchService<Mod>> _tgtLogger;

        public ModPatchServiceBuilder(SourceFileService sourceFileService, FilePatcher filePatcher, BuildContextFactory contextFactory, ILogger<ModPatchService<Mod>> logger)
        {
            _fileService = sourceFileService;
            _filePatcher = filePatcher;
            _ctxFactory = contextFactory;
            _tgtLogger = logger;
        }

        public async Task<ModPatchService<Mod>> GetPatchService(IEnumerable<KeyValuePair<string, Mod>> modCollection, string ctxName = null)
        {
            var mods = modCollection.Select(p => p.Value).ToList();
            var ctx = await _ctxFactory.Create(ctxName);
            return new ModPatchService<Mod>(_filePatcher, _fileService, ctx, mods, _tgtLogger);

        }
        
        public async Task<ModPatchService<Mod>> GetPatchService(IEnumerable<Mod> modCollection, string ctxName = null)
        {
            var mods = modCollection.ToList();
            var ctx = await _ctxFactory.Create(ctxName);
            return new ModPatchService<Mod>(_filePatcher, _fileService, ctx, mods, _tgtLogger);
        }
    }
}