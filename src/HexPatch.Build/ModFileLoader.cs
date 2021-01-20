using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace HexPatch.Build
{
    public class ModFileLoader : IModFileLoader
    {
        private readonly ILogger<ModFileLoader> _logger;

        public ModFileLoader()
        {
            
        }

        public ModFileLoader(ILogger<ModFileLoader> logger) : this()
        {
            _logger = logger;
        }
        public Dictionary<string, Mod> LoadFromFiles(IEnumerable<string> filePaths)
        {
            var fileMods = new Dictionary<string, Mod>();
            foreach (var file in filePaths.Where(f => f.Length > 0 && File.ReadAllText(f).Any()))
            {
                try
                {
                    _logger?.LogTrace($"Attempting to load mod data from {file}");
                    var allText = File.ReadAllText(file);
                    var jsonOpts = new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                        Converters =
                        {
                            new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                        }
                    };
                    if (JsonSerializer.Deserialize<Mod>(allText, jsonOpts) is var jsonMod && jsonMod?.FilePatches != null && jsonMod.FilePatches.Any()) {
                        _logger?.LogTrace($"Successfully loaded mod data from {file}: {jsonMod.GetLabel(Path.GetFileName(file))}");
                        fileMods.Add(file, jsonMod);
                    }
                }
                catch (System.Exception)
                {
                    _logger?.LogWarning($"Failed to load mod data from {file}!");
                }
            }
            return fileMods;
        }
    }
}