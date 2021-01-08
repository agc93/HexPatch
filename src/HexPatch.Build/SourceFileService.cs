using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HexPatch.Build {
    public class SourceFileService
    {
        private readonly SourceFileOptions _opts;
        private readonly List<DirectoryInfo> _sources;
        public SourceFileService(SourceFileOptions buildOpts) {
            _opts = buildOpts;
            _sources = new List<DirectoryInfo>();
            foreach (var fileSource in _opts.FileSources.Where(Directory.Exists)) {
                _sources.Add(new DirectoryInfo(fileSource));
            }
        }

        public FileInfo LocateFile(string fileName) {
            var matchingFiles = _sources.SelectMany(s => {
                return s
                    .EnumerateFiles(Path.GetFileName(fileName), _opts.RecursiveFileSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(f => f.Name == Path.GetFileName(fileName) && f.Exists);
            });
            return matchingFiles.FirstOrDefault();
        }

        public FileInfo GetWorkingFile(string fileName, DirectoryInfo targetPath) {
            
            var file = LocateFile(fileName);
            var targetFile = new FileInfo(Path.Join(targetPath.FullName, file.Name));
            if (!targetFile.Exists) {
                file.CopyTo(targetFile.FullName);                
            }
            
            // NOPE this should return an existing version if it exists since ideally the targetPath will be per-build
            return targetFile.Exists ? targetFile : null;
        }
    }
}