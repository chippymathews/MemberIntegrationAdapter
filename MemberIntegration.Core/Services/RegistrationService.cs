using MemberIntegration.Core.Mapping;
using MemberIntegration.Core.Models.Registration;
using MemberIntegration.Core.Results;
using MemberIntegration.Core.Validation;

namespace MemberIntegration.Core.Services
{
    public sealed class RegistrationService
    {
        private readonly RegistrationValidator _validator;
        private readonly RegistrationMapper _mapper;

        public RegistrationService(RegistrationValidator validator, RegistrationMapper mapper)
        {
            _validator = validator;
            _mapper = mapper;
        }

        public RegistrationResult Process(MemberRegistrationRequest? request)
        {
            var errors = _validator.Validate(request);

            if (errors.Count > 0)
                return RegistrationResult.Failure(errors);

            var payload = _mapper.Map(request!);

            return RegistrationResult.Success(payload);
        }
    }
}
