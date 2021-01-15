using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace HexPatch
{
    public class Mod {
        [JsonPropertyName("_meta")]
        public SetMetadata Metadata {get;set;}
        public Dictionary<string, List<PatchSet>> FilePatches {get;set;}

        public string GetLabel(string defaultValue = null)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(this?.Metadata.DisplayName)) {
                sb.Append(this.Metadata.DisplayName);
            } else if (this.FilePatches.Any(ps => ps.Value.Any(p => !string.IsNullOrWhiteSpace(p.Name)))) {
                sb.Append(this.FilePatches.First(ps => ps.Value.Any(p => !string.IsNullOrWhiteSpace(p.Name))).Value.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Name)).Name);
            }
            if (!string.IsNullOrWhiteSpace(this?.Metadata?.Author)) {
                sb.Append($" (by {this.Metadata.Author})");
            }
            return sb.Length > 0 ? sb.ToString() : defaultValue;
        }
        
    }
}