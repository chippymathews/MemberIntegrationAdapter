using System.Text.Json.Serialization;

namespace MemberIntegration.Core.Models.Legacy
{
    public class LegacyMemberRequest
    {
        [JsonPropertyName("member")]
        public LegacyMember? Member { get; set; }
    }
}
