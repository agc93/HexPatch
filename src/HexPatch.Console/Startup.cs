using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HexPatch.Console {
    public class Startup {
        public static IConfiguration BuildConfiguration(string[] args = null) {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddConfiguration(args ?? new string[0], "pwp_");
            return configBuilder.Build();
        }
    }
}