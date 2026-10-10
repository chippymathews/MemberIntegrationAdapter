using MemberIntegration.Core.Models.Legacy;
using MemberIntegration.Core.Models.Registration;
using System.Globalization;

namespace MemberIntegration.Core.Mapping
{
    public sealed class RegistrationMapper
    {
        public LegacyMemberRequest Map(MemberRegistrationRequest request)
        {
            var dateOfBirth = DateTime.ParseExact(request.DateOfBirth!,"yyyy-MM-dd",CultureInfo.InvariantCulture);

            var planCode = request.MembershipType!.Trim().ToUpperInvariant() switch
            {
                "SINGLE" => "S",
                "FAMILY" => "F",
                "COUPLE" => "C",
                _ => throw new InvalidOperationException("Unsupported membershipType passed validation.")
            };

            return new LegacyMemberRequest
            {
                Member = new LegacyMember
                {
                    GivenName = request.FirstName!.Trim(),
                    FamilyName = request.LastName!.Trim(),
                    Dob = dateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Contact = new LegacyContact
                    {
                        Email = request.Email!.Trim()
                    },
                    PlanCode = planCode,
                    Source = "MILKYWAY"
                }
            };
        }
    }
}