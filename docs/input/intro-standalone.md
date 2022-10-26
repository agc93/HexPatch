Title: Standalone Usage
---

# Using HexPatch Standalone

### Install

First, install the [`HexPatch`](https://www.nuget.org/packages/HexPatch/0.0.0-preview.0.18) package with your preferred package method (NuGet CLI, `dotnet`, Visual Studio, etc).

### Usage

> If you haven't already, you should really read the *Concepts* guide to get an understanding of how patches work!

First, build your patches. We'll reuse the example from the Concepts guide here:

```csharp
var set = new PatchSet<Patch> {
    Name = "Pilot Count",
    Patches = new List<Patch> {
        new Patch {
            Description = "Set Pilot count to 2",
            Template = "01 00 00 00 1C 01",
            Value = "02 00 00 00 1C 01",
            Type = "inPlace"
        }
    }
};
```

Next, we create a `FilePatcher` to handle the actual patching:

```csharp
var patcher = new FilePatcher(null, null);
// you can optionally provide an ILogger<FilePatcher> logger...
var patcher = new FilePatcher(_logger, null);
// ...or custom replacement types
var patcher = new FilePatcher(null, new[] { new MyReplacementType() });
```

Now invoke the patcher with the path to whatever file you're attempting to patch, the patch sets to run on that file, and (optionally) where to write the output file:

```csharp
var result = await patcher.RunPatch(srcFi.FullName, new[] { set }, targetFilePath);
```

The patcher will return a `FileInfo` with the resulting file. If you don't provide a target file path, HexPatch will edit the source file in-place.