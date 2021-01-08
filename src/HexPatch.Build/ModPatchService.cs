using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using Microsoft.Extensions.Logging;

namespace HexPatch.Build {
    public class ModPatchServiceBuilder
    {
        private readonly SourceFileService _fileService;
        private readonly FilePatcher _filePatcher;
        private readonly BuildContextFactory _ctxFactory;
        private readonly ILogger<ModPatchService> _tgtLogger;

        public ModPatchServiceBuilder(SourceFileService sourceFileService, FilePatcher filePatcher, BuildContextFactory contextFactory, ILogger<ModPatchService> logger)
        {
            _fileService = sourceFileService;
            _filePatcher = filePatcher;
            _ctxFactory = contextFactory;
            _tgtLogger = logger;
        }

        public async Task<ModPatchService> GetPatchService(IEnumerable<KeyValuePair<string, Mod>> modCollection)
        {
            var mods = modCollection.ToList();
            var ctx = await _ctxFactory.Create(null);
            return new ModPatchService(_filePatcher, _fileService, ctx, mods, _tgtLogger);

        }
    }
    public class ModPatchService {
        private readonly FilePatcher _patcher;
        private readonly BuildContext _ctx;
        private readonly SourceFileService _fileService;
        private readonly ILogger<ModPatchService> _logger;
        private List<KeyValuePair<string, Mod>> Mods { get; }

        internal ModPatchService(FilePatcher patcher, SourceFileService fileService, BuildContext context, List<KeyValuePair<string, Mod>> mods, ILogger<ModPatchService> logger) {
            _patcher = patcher;
            _ctx = context;
            Mods = mods;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<ModPatchService> RunPatches() {
            foreach (var (dtmFile, mod) in Mods)
            {
                var modifiedFiles = new List<FileInfo>();
                _logger?.LogInformation($"Running patches for {mod.GetLabel(dtmFile)}");
                foreach (var (targetFile, patchSets) in mod.FilePatches)
                {
                    _logger?.LogDebug($"Patching {Path.GetFileName(targetFile)}...");
                    var finalFile = await _patcher.RunPatch(Path.Join(_ctx.WorkingDirectory.FullName, targetFile), patchSets);
                    modifiedFiles.Add(finalFile);
                }
                _logger?.LogDebug($"Modified {modifiedFiles.Count} files: {string.Join(", ", modifiedFiles.Select(f => f.Name))}");
            }
            return this;
        }

        public ModPatchService LoadFiles()
        {
            var requiredFiles = this.Mods
                .SelectMany(em => em.Value.FilePatches)
                .GroupBy(fp => fp.Key)
                .Select(g => g.Key)
                .Distinct()
                .ToList();
            foreach (var file in requiredFiles)
            {
                var srcFile = _fileService.LocateFile(Path.GetFileName(file));
                this._ctx.AddFile(file, srcFile);
            }
            return this;
        }

        public (bool Success, FileInfo Output)? RunBuild(FileInfo targetFile)
        {
            var result = _ctx.BuildScript?.RunBuild(targetFile.FullName);
            return result;
        }
    }
}