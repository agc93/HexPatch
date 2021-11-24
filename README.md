# HexPatch

> A reasonably generic framework for applying arbitrary hex-level edits to files

## What is it?

This is a library/framework used for creating "patches" to binary files. Ultimately, it's just a way of defining, loading and applying any number of byte-level modifications to a binary file. It uses `ModEngine` as its underlying framework and includes a `HexPatchEngine` that can be used with `ModEngine` to quickly drop-in HexPatch functionality.

HexPatch was originally designed for, and is likely most applicable to, scenarios like game modding where manual hex edits to binary files can be hard to track, easy to get wrong and often impossible to combine.

This project also includes `HexPatch.Build`, an additional companion library for `HexPatch` that uses `BuildEngine` to simplify semi-isolated repeatable environments for these edits. You should consider this deprecated in favour of `ModEngine.Build`.