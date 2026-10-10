using MemberIntegration.Core.Mapping;
using MemberIntegration.Core.Models.Registration;
using System.Text.Json;

namespace MemberIntegration.Tests;

public class RegistrationMapperTests
{
    private readonly RegistrationMapper _mapper = new();

    [Fact]
    public void ValidRegistration_MapsToLegacyPayload()
    {
        var payload = _mapper.Map(ValidRegistration());

        var member = payload.Member!;
        Assert.Equal("Alex", member.GivenName);
        Assert.Equal("Nguyen", member.FamilyName);
        Assert.Equal("12/04/1990", member.Dob);
        Assert.Equal("alex.nguyen@example.com", member.Contact!.Email);
        Assert.Equal("S", member.PlanCode);
        Assert.Equal("MILKYWAY", member.Source);

        using var json = JsonDocument.Parse(JsonSerializer.Serialize(payload));
        var legacyMember = json.RootElement.GetProperty("member");
        Assert.Equal("Alex", legacyMember.GetProperty("given_name").GetString());
        Assert.Equal("12/04/1990", legacyMember.GetProperty("dob").GetString());
    }

    [Theory]
    [InlineData("Single", "S")]
    [InlineData("Family", "F")]
    [InlineData("Couple", "C")]
    public void MembershipType_MapsToExpectedPlanCode(string type, string expectedCode)
    {
        var request = ValidRegistration();
        request.MembershipType = type;

        var payload = _mapper.Map(request);

        Assert.Equal(expectedCode, payload.Member!.PlanCode);
    }

    [Fact]
    public void ValidDob_ConvertsToLegacyFormat()
    {
        var request = ValidRegistration();
        request.DateOfBirth = "1990-04-12";

        var payload = _mapper.Map(request);

        Assert.Equal("12/04/1990", payload.Member!.Dob);
    }

    [Fact]
    public void UnexpectedJsonField_DoesNotAffectMapping()
    {
        const string json = """
        {
          "firstName": "Alex",
          "lastName": "Nguyen",
          "dateOfBirth": "1990-04-12",
          "email": "alex.nguyen@example.com",
          "membershipType": "Single",
          "unexpectedField": "ignored"
        }
        """;

        var request = JsonSerializer.Deserialize<MemberRegistrationRequest>(json);

        var payload = _mapper.Map(request!);

        Assert.Equal("Alex", payload.Member!.GivenName);
    }

    private static MemberRegistrationRequest ValidRegistration()
    {
        return new MemberRegistrationRequest
        {
            FirstName = "Alex",
            LastName = "Nguyen",
            DateOfBirth = "1990-04-12",
            Email = "alex.nguyen@example.com",
            MembershipType = "Single"
        };
    }
}