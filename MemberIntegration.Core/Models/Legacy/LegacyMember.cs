using System.Text.Json.Serialization;

namespace MemberIntegration.Core.Models.Legacy
{
    public class LegacyMember
    {
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }

        [JsonPropertyName("dob")]
        public string? Dob { get; set; }

        [JsonPropertyName("contact")]
        public LegacyContact? Contact { get; set; }

        [JsonPropertyName("plan_code")]
        public string? PlanCode { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }
    }
}
