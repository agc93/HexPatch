using System;
using System.Collections.Generic;
using System.Linq;

namespace HexPatch
{
    internal static class ByteExtensions
    {
        internal static byte[] ToByteArray(this string template)
        {
            try {
                template = template.RemoveWhitespace();
                if (!template.IsHex()) {
                    template = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(template)).RemoveWhitespace();
                }
                return Enumerable.Range(0, template.Length / 2).Select(x => Convert.ToByte(template.Substring(x * 2, 2), 16)).ToArray();
            } catch (Exception ex) {
                System.Console.WriteLine(ex.Message);

            }
            return null;
        }

        internal static string RemoveWhitespace(this string input, bool removeSeparator = true)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .Where(c => removeSeparator ? c != '-' : true)
                .ToArray());
        }

        internal static bool IsHex(this IEnumerable<char> chars)
        {
            bool isHex;
            foreach (var c in chars)
            {
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }
            return true;
        }
    }
}