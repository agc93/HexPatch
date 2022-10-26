Title: ModEngine Usage
---

# Using HexPatch with ModEngine

How you set up ModEngine will vary greatly depending on your project, but in short when you register your patch engines (i.e. `IPatchEngine` implementations), you can add a `HexPatchEngine`. Note that if you're using DI, you'll also need to register a `FilePatcher` that the engine will use.

```csharp
var modService =  new YourModPatchService(mods, ctx, _fileService, _modBuilder, _logger);
modService.AddEngine(new HexPatchEngine(_filePatcher, null), mod => mod.FilePatches);
```

Now, when you run your engine, it will loop through any patches it finds in your mods and run them through your patch selector, running any matching patches through HexEngine without you having to directly call it. 