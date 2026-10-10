using MemberIntegration.Core.Models.Legacy;

namespace MemberIntegration.Core.Results
{
    public sealed class RegistrationResult
    {
        public LegacyMemberRequest? Payload { get; }
        public IReadOnlyList<string> Errors { get; }
        public bool IsSuccess => Payload is not null && Errors.Count == 0;

        private RegistrationResult(LegacyMemberRequest? payload, IReadOnlyList<string> errors)
        {
            Payload = payload;
            Errors = new List<string>(errors).AsReadOnly();
        }

        public static RegistrationResult Success(LegacyMemberRequest payload)
        {
            return new RegistrationResult(payload, []);
        }

        public static RegistrationResult Failure(IReadOnlyList<string> errors)
        {
            return new RegistrationResult(null, errors);
        }
    }
}
