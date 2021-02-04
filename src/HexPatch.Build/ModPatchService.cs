using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using Microsoft.Extensions.Logging;

namespace HexPatch.Build {
    public class ModPatchService<TMod> : IDisposable where TMod : Mod {
        protected readonly FilePatcher _patcher;
        protected readonly BuildContext _ctx;
        protected readonly SourceFileService _fileService;
        protected readonly ILogger<ModPatchService<TMod>> _logger;
        public List<TMod> Mods { get; }
        public Func<BuildContext, FileInfo> PreBuildAction { get; set; }

        internal protected ModPatchService(FilePatcher patcher, SourceFileService fileService, BuildContext context, IEnumerable<TMod> mods, ILogger<ModPatchService<TMod>> logger) {
            _patcher = patcher;
            _ctx = context;
            Mods = mods.ToList();
            _fileService = fileService;
            _logger = logger;
        }

        public virtual async Task<ModPatchService<TMod>> RunPatches() {
            foreach (var mod in Mods)
            {
                var modifiedFiles = new List<FileInfo>();
                _logger?.LogInformation($"Running patches for {mod.GetLabel()}");
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

        public virtual ModPatchService<TMod> LoadFiles(Func<string, IEnumerable<string>> extraFileSelector = null)
        {
            var requiredFiles = this.Mods
                .SelectMany(em => em.FilePatches)
                .GroupBy(fp => fp.Key)
                .Select(g => g.Key)
                .Distinct()
                .ToList();
            foreach (var file in requiredFiles)
            {
                var srcFile = _fileService.LocateFile(Path.GetFileName(file));
                this._ctx.AddFile(Path.GetDirectoryName(file), srcFile);
                if (extraFileSelector != null) {
                    var extraFiles = extraFileSelector.Invoke(file) ?? new List<string>();
                    foreach (var eFile in extraFiles) {
                        var exFile = _fileService.LocateFile(Path.GetFileName(eFile));
                        this._ctx.AddFile(Path.GetDirectoryName(eFile), exFile);
                    }
                }
            }
            return this;
        }

        public virtual (bool Success, FileInfo Output)? RunBuild(FileInfo targetFile)
        {
            var bResult = PreBuildAction?.Invoke(_ctx);
            if (bResult != null && bResult.Exists)
            {
                targetFile = bResult;
            }
            var result = _ctx.BuildScript?.RunBuild(targetFile.FullName);
            return ((bool Success, FileInfo Output)?)(result ?? (bResult != null ? (true, bResult) : null));
        }

        public virtual (bool Success, FileInfo Output)? RunBuild(Func<BuildContext, FileInfo> targetFileFunc)
        {
            var target = targetFileFunc.Invoke(_ctx);
            return RunBuild(target);
        }

        public virtual (bool Success, T Output)? RunAction<T>(Func<BuildContext, T> buildFunc) where T : class
        {
            try
            {
                var result = buildFunc.Invoke(_ctx);
                return (true, result);
            }
            catch (Exception e)
            {
                _logger?.LogWarning("Error encountered during patch action!", e);
                return (false, null);
            }
        }

        public virtual void Dispose()
        {
            ((IDisposable)_ctx).Dispose();
        }
    }
}