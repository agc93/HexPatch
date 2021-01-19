using System;
using System.Collections.Generic;
using System.Linq;

namespace HexPatch
{
    internal static class ByteExtensions
    {
        private static Dictionary<string, Func<string, byte[]>> _converters = new Dictionary<string, Func<string, byte[]>> {
            ["int:"] = s => BitConverter.GetBytes(int.Parse(s)),
            ["text:"] = s => System.Text.Encoding.UTF8.GetBytes(s)
        };
        internal static byte[] ToByteArray(this string template)
        {
            try {
                if (_converters.FirstOrDefault(c => template.StartsWith(c.Key)) is var conv && conv.Value != null) {
                    return conv.Value.Invoke(template.Replace(conv.Key, string.Empty));
                }
                // template = template.RemoveWhitespace();
                // if (!template.IsHex() || template.StartsWith("text:")) {
                /* if (template.StartsWith("text:")) {
                    template = template.Replace("text:", string.Empty);
                    template = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(template));
                } */
                template = template.RemoveWhitespace();
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
            foreach (var c in chars.Where(c => c != '-'))
            {
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }
            return true;
        }

        internal static bool HasValue(this string s) {
            return !string.IsNullOrWhiteSpace(s);
        }

        internal static T TryGetNext<T>(this IEnumerable<T> collection, int index, T defaultValue) {
            return collection.ElementAtOrDefault(index + 1) ?? defaultValue;
        }
    }
}