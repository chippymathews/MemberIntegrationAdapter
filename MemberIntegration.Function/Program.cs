using MemberIntegration.Core.Mapping;
using MemberIntegration.Core.Services;
using MemberIntegration.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<RegistrationValidator>();
        services.AddSingleton<RegistrationMapper>();
        services.AddSingleton<RegistrationService>();
    })
    .Build();

host.Run();

