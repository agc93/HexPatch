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
        internal record BreakPattern {
            internal IReadOnlyCollection<byte> Pattern {get; init;} = new byte[0];
            internal int? Offset {get;init;}
        }
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
                var changes = set.Patches
                    .Where(p => p.Type == SubstitutionType.Before)
                    .SelectMany(p => PatternAt(fileBytes, p)
                                        .Select(o => new KeyValuePair<int, byte[]>(o, p.Substitution.ToByteArray()))
                    );
                var finalBytes = ReplaceBytesBefore(fileBytes, changes);
                var replacements = set.Patches
                    .Where(p => p.Type == SubstitutionType.InPlace)
                    .SelectMany(p => PatternAt(fileBytes, p)
                                    .Select(o => new ByteReplacement {
                                        MatchOffset = o,
                                        Key = p.Template.ToByteArray(),
                                        Replacement = p.Substitution.ToByteArray()
                                    })
                );
                finalBytes = ReplaceBytes(finalBytes, replacements);
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
            var targetName = Path.GetFileName(sourcePath.FullName);
            
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

        public static byte[] ReplaceBytes(byte[] src, IEnumerable<ByteReplacement> replacements)
    {
        var dst = new byte[src.Length + Convert.ToInt32(Math.Abs(src.Length*0.25))];
        var lastIndex = 0;
        replacements = replacements.OrderBy(r => r.MatchOffset);
        var srcStream = new MemoryStream(src);
        var tgtStream = new MemoryStream(dst);
        foreach (var replacement in replacements)
        {
            var tgt = new List<byte[]>();
            var unchanged = Convert.ToInt32(replacement.MatchOffset - srcStream.Position);
            // before found array
            srcStream.CopyToStream(tgtStream, unchanged);
            tgtStream.Write(replacement.Replacement);
            srcStream.Seek(replacement.Key.Length, SeekOrigin.Current);

            lastIndex = (int)srcStream.Position;
        }
        srcStream.CopyToStream(tgtStream, src.Length - lastIndex);
        var finalLength = tgtStream.Position;
        var final = new byte[finalLength];
        tgtStream.Seek(0, SeekOrigin.Begin);
        tgtStream.Read(final, 0, (int)finalLength);
        // Array.Resize(ref dst, finalLength);

        return final;
    }

    private static IEnumerable<int> PatternAt(IReadOnlyCollection<byte> source, Patch patch) {
        IEnumerable<int> GetPatterns(IReadOnlyCollection<byte> pattern, Func<int, int>? skipFunc = null, BreakPattern? breakPattern = null) {
            skipFunc ??= (i) => i;
            for (var i = 0; i < source.Count; i++)
            {
                var current = source.Skip(skipFunc(i)).Take(pattern.Count);
                if (breakPattern != null && breakPattern.Pattern.Any() && current.SequenceEqual(breakPattern.Pattern)) {
                    break;
                }
                if (breakPattern != null && breakPattern.Offset != null && skipFunc(i) == breakPattern.Offset) {
                    break;
                }
                if (current.SequenceEqual(pattern))
                {
                    yield return skipFunc(i);
                }
            }
        }
        if (patch.Window?.After == null) {
            return GetPatterns(patch.Template.ToByteArray());
        } else if (!string.IsNullOrWhiteSpace(patch.Window?.After) && !string.IsNullOrWhiteSpace(patch.Window?.Before)) {
            //both sides of the window set
            var openBounds = GetPatterns(patch.Window.After.ToByteArray()).ToList();
            //this is a fucking disaster
            //but seems to work
            var windows = openBounds.Aggregate(new List<(int StartOffset, int EndOffset)>(), (acc, current) => {
                var end = GetPatterns(patch.Window.Before.ToByteArray(), i => current + i, new BreakPattern { Offset = openBounds.TryGetNext(openBounds.IndexOf(current), int.MaxValue)}).ToList();
                if (end.Any()) {
                    acc.Add((current, end.First()));
                };
                return acc;
                // return (start, end.First());
            }, acc => acc.ToList());
            var allMatches = windows.SelectMany((window, idx) => {
                return GetPatterns(patch.Template.ToByteArray(), i => window.StartOffset + i, new BreakPattern { Pattern = patch.Window.After.ToByteArray(), Offset = window.EndOffset});
            }).ToList();
            return allMatches;
            /*
            (for finding pattern X between A and B)
            This just takes every instance of A, and creates a window from there to the next occurrence of B (unless it find another A)
            So we get a set of windows ending at B, starting from the *nearest* instance of A. These windows may or may not include X
            We then search those windows for X, returning every offset of X from anywhere in each window.
            */
        } else if (!string.IsNullOrWhiteSpace(patch.Window?.After)) {
            //so this doubles up
            // if you ask for a pattern after A (with no end bound) but A appears twice
            // then it will start looking at A again
            // we get around this by baioling out of GetPatterns if we hit the next instance of A
            var startPatterns = GetPatterns(patch.Window.After.ToByteArray()).ToList();
            var matchPatterns = startPatterns.Select((start, idx) => GetPatterns(patch.Template.ToByteArray(), i => start + i, new BreakPattern {Offset = startPatterns.TryGetNext(idx, int.MaxValue)})).ToList();
            var final = matchPatterns.SelectMany(i => i).ToList();
            return final;
        }
        return new int[0];
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

    public record ByteReplacement {
        public int MatchOffset {get;init;}
        public byte[] Key {get;init;}
        public byte[] Replacement {get; init;}
    }
}