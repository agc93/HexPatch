using Statiq.App;
using Statiq.Docs;

await Bootstrapper
    .Factory
    .CreateDocs(args)
    .DeployToGitHubPages(
        "agc93",
        "HexPatch",
        Config.FromSetting<string>("GITHUB_TOKEN")
    )
    .RunAsync();