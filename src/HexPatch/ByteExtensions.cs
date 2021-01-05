using System;
using System.Linq;

namespace HexPatch
{
    internal static class ByteExtensions
    {
        internal static byte[] ToByteArray(this string template)
        {
            try {
                template = template.RemoveWhitespace();
                return Enumerable.Range(0, template.Length / 2).Select(x => Convert.ToByte(template.Substring(x * 2, 2), 16)).ToArray();
            } catch (Exception ex) {
                System.Console.WriteLine(ex.Message);

            }
            return null;
        }

        internal static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}