using System.Collections.Generic;

namespace HexPatch.Build
{
    /// <summary>
    /// This is purely a convenience interface for abstracting the process of "loading" mods from files.
    /// You do not need to implement this if it's not needed, but it can be convenient.
    /// </summary>
    public interface IModFileLoader
    {
        Dictionary<string, Mod> LoadFromFiles(IEnumerable<string> filePaths);
    }
}