using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace HexPatch.Console {
    public static class HostBuilderExtensions
    {
        public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder config, string[] args, string variablePrefix = null) {
            // config.AddConfigFile("config");
            config.AddConfigFile("config", true);
            /*if (args != null && args.Any())
            {
                config.AddCommandLine(args);
            }*/
            if (string.IsNullOrWhiteSpace(variablePrefix)) {
                config.AddEnvironmentVariables();
            } else {
                config.AddEnvironmentVariables(variablePrefix);
            }
            return config;
        }

        public static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder configBuilder, string path, bool allVariants = true) {
            var formats = new Dictionary<string, Action<IConfigurationBuilder, string>> {
                [".json"] = (builder, p) => builder.AddJsonFile(p, true, true),
                [".conf"] = (builder, p) => builder.AddJsonFile(p, true, true),
                /*[".yml"] = (builder, p) => builder.AddYamlFile(p, true, true),
                [".yaml"] = (builder, p) => builder.AddYamlFile(p, true, true)*/
            };
            path = Path.IsPathRooted(path) ? path : Path.GetFullPath(path);
            if (Path.HasExtension(path)) {
                var format = formats.TryGetValue(Path.GetExtension(path), out var confAction);
                if (format) {
                    confAction.Invoke(configBuilder, path);
                }
            } else {
                path = path.TrimEnd('.');
                if (allVariants) {
                    foreach (var format in formats)
                    {
                        format.Value.Invoke(configBuilder, path + format.Key);
                    }
                } else {
                    var matchedFiles = formats.Where(f => File.Exists(path + f.Key));
                    if (matchedFiles.Any()) {
                        foreach (var match in matchedFiles)
                        {
                            match.Value.Invoke(configBuilder, path + match.Key);
                        }
                    }
                }
            }
            return configBuilder;
        }
    }
}