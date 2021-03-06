﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using Microsoft.Extensions.Logging;

namespace HexPatch.Build {
    public class ModPatchService<TMod> : BuildService<DirectoryBuildContext>, IDisposable where TMod : Mod {
        protected readonly FilePatcher _patcher;
        protected readonly ISourceFileService _fileService;
        protected readonly ILogger<ModPatchService<TMod>> _logger;
        private readonly IModBuilder _delegate;
        public List<TMod> Mods { get; }
        public Func<DirectoryBuildContext, FileInfo> PreBuildAction { get; set; }

        internal protected ModPatchService(FilePatcher patcher, ISourceFileService fileService, DirectoryBuildContext context, IModBuilder @delegate, IEnumerable<TMod> mods, ILogger<ModPatchService<TMod>> logger) : base(context) {
            _patcher = patcher;
            Mods = mods.ToList();
            _fileService = fileService;
            _logger = logger;
            _delegate = @delegate;
        }

        public virtual async Task<ModPatchService<TMod>> RunPatches() {
            foreach (var mod in Mods)
            {
                var modifiedFiles = new List<FileInfo>();
                _logger?.LogInformation($"Running patches for {mod.GetLabel()}");
                foreach (var (targetFile, patchSets) in mod.FilePatches)
                {
                    _logger?.LogDebug($"Patching {Path.GetFileName(targetFile)}...");
                    var finalFile = await _patcher.RunPatch(Path.Join(Context.WorkingDirectory.FullName, targetFile), patchSets);
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
                .Where(g => g.Any())
                .Select(g => g.Key)
                .Distinct()
                .ToList();
            foreach (var file in requiredFiles)
            {
                var srcFile = _fileService.LocateFile(Path.GetFileName(file));
                this.BuildContext.AddFile(Path.GetDirectoryName(file), srcFile);
                if (extraFileSelector != null) {
                    var extraFiles = extraFileSelector.Invoke(file) ?? new List<string>();
                    foreach (var eFile in extraFiles) {
                        var exFile = _fileService.LocateFile(Path.GetFileName(eFile));
                        this.BuildContext.AddFile(Path.GetDirectoryName(eFile), exFile);
                    }
                }
            }
            return this;
        }

        public virtual async Task<(bool Success, FileInfo Output)?> RunBuild(FileInfo targetFile)
        {
            var bResult = PreBuildAction?.Invoke(Context);
            if (bResult is {Exists: true})
            {
                targetFile = bResult;
            }
            
            var result = await _delegate.RunBuildAsync(BuildContext, targetFile.FullName);
            return ((bool Success, FileInfo Output)?)result;
        }

        public virtual async Task<(bool Success, FileInfo Output)?> RunBuild(string targetFileName) {
            var target = Path.IsPathRooted(targetFileName)
                ? new FileInfo(targetFileName)
                : new FileInfo(Path.Combine(Context.WorkingDirectory.FullName, targetFileName));
            return await RunBuild(target);
        }

        public virtual (bool Success, T Output)? RunAction<T>(Func<DirectoryBuildContext, T> buildFunc) where T : class
        {
            try
            {
                var result = buildFunc.Invoke(Context);
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
            BuildContext.Dispose();
        }

        public override async Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(string targetFileName) {
            return await _delegate.RunBuildAsync(BuildContext, targetFileName);
        }
    }
}