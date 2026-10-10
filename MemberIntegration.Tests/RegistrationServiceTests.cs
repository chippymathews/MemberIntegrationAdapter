using MemberIntegration.Core.Mapping;
using MemberIntegration.Core.Models.Registration;
using MemberIntegration.Core.Services;
using MemberIntegration.Core.Validation;
using System.Text.Json;

namespace MemberIntegration.Tests;

public class RegistrationServiceTests
{
    private readonly RegistrationService _service = new(new RegistrationValidator(), new RegistrationMapper());

    [Fact]
    public void ValidRegistration_ReturnsMappedPayload()
    {
        var result = _service.Process(ValidRegistration());

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);

        var member = result.Payload!.Member!;
        Assert.Equal("Alex", member.GivenName);
        Assert.Equal("Nguyen", member.FamilyName);
        Assert.Equal("12/04/1990", member.Dob);
        Assert.Equal("alex.nguyen@example.com", member.Contact!.Email);
        Assert.Equal("S", member.PlanCode);
        Assert.Equal("MILKYWAY", member.Source);
    }

    [Fact]
    public void NullRegistration_ReturnsError()
    {
        var result = _service.Process(null);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Payload);
        Assert.Contains("Registration is required.", result.Errors);
    }

    [Theory]
    [InlineData("FirstName", "firstName", null)]
    [InlineData("FirstName", "firstName", "")]
    [InlineData("FirstName", "firstName", " ")]
    [InlineData("LastName", "lastName", null)]
    [InlineData("LastName", "lastName", "")]
    [InlineData("LastName", "lastName", " ")]
    [InlineData("DateOfBirth", "dateOfBirth", null)]
    [InlineData("DateOfBirth", "dateOfBirth", "")]
    [InlineData("DateOfBirth", "dateOfBirth", " ")]
    [InlineData("Email", "email", null)]
    [InlineData("Email", "email", "")]
    [InlineData("Email", "email", " ")]
    [InlineData("MembershipType", "membershipType", null)]
    [InlineData("MembershipType", "membershipType", "")]
    [InlineData("MembershipType", "membershipType", " ")]
    public void MissingOrBlankRequiredField_ReturnsError(string property, string errorField, string? value)
    {
        var request = ValidRegistration();

        SetField(request, property, value);

        var result = _service.Process(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Contains(errorField, StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("alex@")]
    [InlineData("alex smith@example.com")]
    public void InvalidEmail_ReturnsError(string email)
    {
        var request = ValidRegistration();
        request.Email = email;

        var result = _service.Process(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Contains("email", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void UnsupportedMembershipType_ReturnsError()
    {
        var request = ValidRegistration();
        request.MembershipType = "Student";

        var result = _service.Process(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Contains("membershipType", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("12/04/1990")]
    [InlineData("1990-13-45")]
    public void InvalidDobFormat_ReturnsError(string dateOfBirth)
    {
        var request = ValidRegistration();
        request.DateOfBirth = dateOfBirth;

        var result = _service.Process(request);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Contains("dateOfBirth", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MultipleInvalidFields_ReturnMultipleErrors()
    {
        var request = new MemberRegistrationRequest();

        var result = _service.Process(request);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Payload);
        Assert.Contains("firstName is required.", result.Errors);
        Assert.Contains("lastName is required.", result.Errors);
        Assert.Contains("dateOfBirth is required.", result.Errors);
        Assert.Contains("email is required.", result.Errors);
        Assert.Contains("membershipType is required.", result.Errors);
    }

    [Fact]
    public void UnexpectedJsonField_DoesNotAffectProcessing()
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

        var result = _service.Process(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Alex", result.Payload!.Member!.GivenName);
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

    private static void SetField(MemberRegistrationRequest request, string property, string? value)
    {
        switch (property)
        {
            case "FirstName": request.FirstName = value; break;
            case "LastName": request.LastName = value; break;
            case "DateOfBirth": request.DateOfBirth = value; break;
            case "Email": request.Email = value; break;
            case "MembershipType": request.MembershipType = value; break;
        }
    }
}