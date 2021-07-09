using System.IO;

namespace HexPatch.Build
{
    public interface ISourceFileService
    {
        FileInfo LocateFile(string fileName);
    }
}