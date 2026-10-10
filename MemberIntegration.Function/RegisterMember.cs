using MemberIntegration.Core.Models.Registration;
using MemberIntegration.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MemberIntegration.Function;

public sealed class RegisterMember
{
    private readonly RegistrationService _registrationService;
    private readonly ILogger<RegisterMember> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public RegisterMember(RegistrationService registrationService, ILogger<RegisterMember> logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

    [Function("RegisterMember")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request)
    {
        MemberRegistrationRequest? registration;

        try
        {
            registration = await JsonSerializer.DeserializeAsync<MemberRegistrationRequest>(request.Body, JsonOptions);
        }
        catch (JsonException)
        {
            _logger.LogWarning("Registration request contained invalid JSON.");
            return new BadRequestObjectResult(new
            {
                errors = new[] { "Request body must contain valid JSON." }
            });
        }

        var result = _registrationService.Process(registration);

        if (!result.IsSuccess)
        {
            _logger.LogInformation("Registration failed validation with {ErrorCount} errors.", result.Errors.Count);

            return new BadRequestObjectResult(new
            {
                errors = result.Errors
            });
        }

        _logger.LogInformation("Registration was mapped successfully.");
        return new OkObjectResult(result.Payload);
    }
}