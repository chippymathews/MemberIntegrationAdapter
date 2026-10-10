using System.Text.Json.Serialization;

namespace MemberIntegration.Core.Models.Registration
{
    public class MemberRegistrationRequest
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public string? DateOfBirth { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("membershipType")]
        public string? MembershipType { get; set; }

        [JsonPropertyName("registeredAt")]
        public DateTimeOffset? RegisteredAt { get; set; }
    }
}