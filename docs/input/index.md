Title: HexPatch Documentation
---

# HexPatch

### What is it?

This is a library/framework used for creating "patches" to binary files. Ultimately, it's just a way of defining, loading and applying any number of byte-level modifications to a binary file. HexPatch was originally designed for, and is likely most applicable to, scenarios like game modding where manual hex edits to binary files can be hard to track, easy to get wrong and often impossible to combine.

### Components

To make it a bit more versatile, HexPatch can be used in a few different contexts: on its own, as a build library or as an engine with [ModEngine](https://github.com/agc93/ModEngine).

If you're just wanting a predictable way of applying a few edits, especially hard-coded ones, then it will work fine on its own. 

If you want a bit more power and flexibility, using the companion `HexPatch.Build` project uses [BuildEngine](https://github.com/DevelopEngine/BuildEngine) to simplify semi-isolated repeatable environments. While this works, the better option in recent versions is using ModEngine (with ModEngine.Build).

Using HexPatch with ModEngine allows for the most flexibility and capabilities, meaning you can create more sophisticated projects without worrying so much about the underlying binary editing. This also makes it much easier to add other (non-binary) patch types, so its better for things like modding.

