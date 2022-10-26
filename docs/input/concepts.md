---

# Basics of HexPatch

## Patch Structure

HexPatch uses ModEngine's patch structure which consists of the following rough design:

- One or more `PatchSet`s
  - which contain one or more `Patch`es
    - where each `Patch` has a `Template`, a `Value` and a `Type`

For example, here's a basic patch set initialization example:

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

Now let's look at those in more detail.

The `PatchSet` is a purely logical organization idea: they're _mostly_ just a way of organizing related patches together. The `PatchSet` object just has two things: a `Name` and a list of `Patch` objects. On it's own, a `PatchSet` doesn't do anything, that's up to the actual `Patch` objects in the set.

The `Patch` is the actual hex edit to make. In this case, we're telling HexPatch to find any instance of the bytes represented as `01 00 00 00 1C 01` in the file and *replace* them with `02 00 00 00 1C 01`. We'll come back to `Type` further on.

When HexPatch runs, we'll tell it where to find our files, give it this patch set and it will open the files, handle all the boring binary handling for us, and just run our patch over those files.

### Patch Types

The patch `Type` is what controls what HexPatch *does* when it finds a match to our `Template`. In the example above, we used `inPlace` which is the name of the `InPlaceReplacementType` built into HexPatch. The `inPlace` type (as the name suggests) replaces the bytes matched by `Template` with the bytes from `Value`. 

> Put another way, we're telling HexPatch to *find* the bytes from `Template`, then *do* whatever the `Type` says with the `Value`.

For another example, there's also a `BeforeReplacementType` which can be used to replace the bytes immediately **before** the `Template` with the value from `Value`. We could have written the patch from above this way instead:

```csharp
// trimmed
new Patch {
    Description = "Set Pilot count to 2",
    Template = "00 00 00 1C 01",
    Value = "02",
    Type = "before"
}
```

These two patches do the same thing! It's all about finding the patch type for what you need to do. You can also add your own patch types by implementing the `IReplacementType` interface!