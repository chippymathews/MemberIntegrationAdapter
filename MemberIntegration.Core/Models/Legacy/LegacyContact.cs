using System.Text.Json.Serialization;

namespace MemberIntegration.Core.Models.Legacy
{
    public class LegacyContact
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
