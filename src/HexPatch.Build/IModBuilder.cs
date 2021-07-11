using System.IO;
using System.Threading.Tasks;
using BuildEngine;

namespace HexPatch.Build
{
    public interface IModBuilder
    {
        public Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(IBuildContext buildContext, string targetFileName);
    }
}