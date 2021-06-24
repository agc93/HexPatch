using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using BuildEngine;
using HexPatch.Build;
using static System.Console;

namespace HexPatch.Console
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var aoa = new PatchSet() {
                Name = "AoA Unlocker",
                Patches = new System.Collections.Generic.List<Patch> {
                new Patch {
                    Description = "Unlocks AoA",
                    Substitution = "01",
                    Template = "004802"
                }
            }};
            var unlock = new PatchSet {
                Name = "Unlock all Planes",
                Patches = new List<Patch> {
                    new Patch {
                        Description = "All Planes available",
                        Template = "00 3E 02",
                        Substitution = "01"
                    },
                    new Patch {
                        Description = "All planes unlocked",
                        Template = "00 D7 01",
                        Substitution = "01"
                    }
                }
            };
            var prez = new PatchSet {
                Name = "Prez Everywhere",
                Patches = new List<Patch> {
                    new Patch {
                        Description = "Set Pilot count to 2",
                        Template = "00 00 00 1C 01",
                        Substitution = "02"
                    }
                }
            };
            var fileService = new SourceFileService(new SourceFileOptions() {
                FileSources = new List<string> {
                    @"X:\ProjectWingman\Dumped\Data"
                },
                RecursiveFileSearch = true
            });
            var sets = new List<PatchSet> {prez, aoa};
            var allFiles = Directory.EnumerateFiles("C:/Users/alist/OneDrive/Source/HexPatch", "*.dtm", SearchOption.TopDirectoryOnly);
            var fileMods = new ModFileLoader().LoadFromFiles(allFiles).ToList();
            // var builder = new ModPatchServiceBuilder(fileService, new FilePatcher(null))
            var patcher = new FilePatcher(null, null);
            var enabledMods = Sharprompt.Prompt.MultiSelect<KeyValuePair<string, Mod>>("Choose the patch mods you'd like to apply", fileMods, 10, 0, -1, m => GetLabel(m.Value) ?? Path.GetFileNameWithoutExtension(m.Key));
            var requiredFiles = enabledMods
                .SelectMany(em => em.Value.FilePatches)
                .GroupBy(fp => fp.Key)
                .Select(g => g.Key)
                .ToList();
            WriteLine($"Loaded {enabledMods.Count()} mods for {requiredFiles.Count} game files");
            foreach (var (modFile, patchMod) in enabledMods)
            {
                WriteLine($"Running patches for {modFile}");
                foreach (var (targetFile, patchSets) in patchMod.FilePatches) {
                    WriteLine($"Patching {Path.GetFileName(targetFile)}...");
                    var srcFi = fileService.GetWorkingFile(targetFile, new DirectoryInfo(@"X:\ProjectWingman\DataTables\Working"));
                    var result = await patcher.RunPatch(srcFi.FullName, patchSets);
                }
            }
            // var result = await patcher.RunPatch(@"X:\ProjectWingman\Dumped\Data\AircraftData\DB_Aircraft.uexp", sets);
            // WriteLine(result.FullName);
        }

        internal static string GetLabel(Mod mod) {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(mod?.Metadata.DisplayName)) {
                sb.Append(mod.Metadata.DisplayName);
            } else if (mod.FilePatches.Any(ps => ps.Value.Any(p => !string.IsNullOrWhiteSpace(p.Name)))) {
                sb.Append(mod.FilePatches.First(ps => ps.Value.Any(p => !string.IsNullOrWhiteSpace(p.Name))).Value.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Name)).Name);
            }
            if (!string.IsNullOrWhiteSpace(mod?.Metadata?.Author)) {
                sb.Append($" (by {mod.Metadata.Author})");
            }
            return sb.Length > 0 ? sb.ToString() : null;
        }
    }

    
}
