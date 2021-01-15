using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HexPatch
{
    public class FilePatcher
    {
        private readonly ILogger<FilePatcher> _logger;

        public FilePatcher(ILogger<FilePatcher> logger)
        {
            _logger = logger;
        }

        public async Task<FileInfo> RunPatch(string sourcePath, IEnumerable<PatchSet> sets, string? targetFilePath = null)
        {
            var fi = new FileInfo(sourcePath);
            var finalTarget = GetTarget(fi, targetFilePath);
            var fileBytes = await File.ReadAllBytesAsync(fi.FullName);
            foreach (var set in sets)
            {
                _logger?.LogInformation($"Running patches for '{set.Name}'");
                // fileBytes = ;
                System.Console.WriteLine($"Running patches for {set.Name}");
                System.Console.WriteLine($"Contains the following patches: {string.Join(", ", set.Patches.Select(p => p.Description))}");
                var changes = set.Patches.Where(p => p.Type == SubstitutionType.Before).SelectMany(p => PatternAt(fileBytes, p.Template.ToByteArray()).Select(o => new KeyValuePair<int, byte[]>(o, p.Substitution.ToByteArray())));
                var finalBytes = ReplaceBytesBefore(fileBytes, changes);
                /* foreach (var patch in set.Patches)
                {
                    var allMatches = PatternAt(fileBytes, patch.Template.ToByteArray());
                    // var changes = patch.
                    foreach (var match in allMatches)
                    {
                        var replacement = patch.Substitution.ToByteArray();
                        // var modArray = ReplaceBytes()
                    }
                } */
                fileBytes = finalBytes;
            }
            await File.WriteAllBytesAsync(finalTarget, fileBytes);
            return new FileInfo(finalTarget);
        }

        private static string GetTarget(FileInfo sourcePath, string? targetPath)
        {
            var sourceDir = sourcePath.Directory?.FullName;
            if (!string.IsNullOrWhiteSpace(targetPath)) {
                return Path.IsPathRooted(targetPath) ? targetPath : !string.IsNullOrWhiteSpace(sourceDir) ? Path.Combine(sourceDir , targetPath) : targetPath;                
            }

            var targetName =
                $"{Path.GetFileNameWithoutExtension(sourcePath.FullName)}.mod{Path.GetExtension(sourcePath.FullName)}";
            return string.IsNullOrWhiteSpace(sourceDir) ? targetName : Path.Combine(sourceDir, targetName);
        }

        private byte[] ReplaceBytesBefore(byte[] src, IEnumerable<KeyValuePair<int, byte[]>> replacements)
        {
            var dst = new byte[src.Length];
            var lastIndex = 0;
            var nextByte = 0;
            replacements = replacements.OrderBy(r => r.Key);
            foreach (var (key, replacement) in replacements)
            {
                // var changeOffset = repl.Key - lastIndex - repl.Value.Length;
                var unchanged = key - nextByte;
                // before found array
                Buffer.BlockCopy(src, nextByte, dst, nextByte, unchanged);
                // repl copy
                Buffer.BlockCopy(replacement, 0, dst, key - replacement.Length, replacement.Length);
                lastIndex = key;
                nextByte = key;
            }
            Buffer.BlockCopy(
                    src,
                    lastIndex,
                    dst,
                    lastIndex,
                    src.Length - lastIndex);
            return dst;
        }

        private static IEnumerable<int> PatternAt(IReadOnlyCollection<byte> source, IReadOnlyCollection<byte> pattern)
        {
            for (var i = 0; i < source.Count; i++)
            {
                if (source.Skip(i).Take(pattern.Count).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }
    }
}