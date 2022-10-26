using Statiq.App;
using Statiq.Docs;

await Bootstrapper
    .Factory
    .CreateDocs(args)
    // .AddSourceFiles("../../src/**/*.cs")
    .RunAsync();