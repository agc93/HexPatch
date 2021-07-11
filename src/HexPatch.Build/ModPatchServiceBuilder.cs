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
    /// <remarks>This is performing the work of an `IBuildServiceProvider`</remarks>
    /// </summary>
    public class ModPatchServiceBuilder
    {
        private readonly ISourceFileService _fileService;
        private readonly FilePatcher _filePatcher;
        private readonly DirectoryBuildContextFactory _ctxFactory;
        private readonly ILogger<ModPatchService<Mod>> _tgtLogger;
        private readonly IModBuilder _modBuilder;

        public ModPatchServiceBuilder(ISourceFileService sourceFileService, FilePatcher filePatcher, DirectoryBuildContextFactory ctxFactory, ILogger<ModPatchService<Mod>> logger, IModBuilder modBuilder)
        {
            _fileService = sourceFileService;
            _filePatcher = filePatcher;
            _ctxFactory = ctxFactory;
            _tgtLogger = logger;
            _modBuilder = modBuilder;
        }

        public async Task<ModPatchService<Mod>> GetPatchService(IEnumerable<KeyValuePair<string, Mod>> modCollection, string ctxName = null)
        {
            var mods = modCollection.Select(p => p.Value).ToList();
            var buildService = _ctxFactory.CreateContext(ctxName);
            return new ModPatchService<Mod>(_filePatcher, _fileService, buildService, _modBuilder, mods, _tgtLogger);

        }
        
        public async Task<ModPatchService<Mod>> GetPatchService(IEnumerable<Mod> modCollection, string ctxName = null)
        {
            var mods = modCollection.ToList();
            var buildService = _ctxFactory.CreateContext(ctxName);
            return new ModPatchService<Mod>(_filePatcher, _fileService, buildService, _modBuilder, mods, _tgtLogger);
        }
    }
}