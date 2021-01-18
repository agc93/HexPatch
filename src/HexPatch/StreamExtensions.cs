using System;
using System.IO;

namespace HexPatch
{
    public static class StreamExtensions
    {
        public static void CopyToStream(this Stream input, Stream output, int bytes)
        {
            byte[] buffer = new byte[65536];
            int read;
            while (bytes > 0 && 
                (read = input.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
            {
                output.Write(buffer, 0, read);
                bytes -= read;
            }
        }
    }
}