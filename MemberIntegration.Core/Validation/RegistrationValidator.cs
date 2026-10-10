using MemberIntegration.Core.Models.Registration;
using System.Globalization;
using System.Net.Mail;

namespace MemberIntegration.Core.Validation
{
    public sealed class RegistrationValidator
    {
        public IReadOnlyList<string> Validate(MemberRegistrationRequest? request)
        {
            var errors = new List<string>();

            if (request is null)
            {
                errors.Add("Registration is required.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add("firstName is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add("lastName is required.");

            if (string.IsNullOrWhiteSpace(request.DateOfBirth))
            {
                errors.Add("dateOfBirth is required.");
            }
            else if (!DateTime.TryParseExact(request.DateOfBirth,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out _))
            {
                errors.Add("dateOfBirth must use the yyyy-MM-dd format.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("email is required.");
            }
            else if (!IsValidEmail(request.Email))
            {
                errors.Add("email must be a plausible email address.");
            }

            if (string.IsNullOrWhiteSpace(request.MembershipType))
            {
                errors.Add("membershipType is required.");
            }
            else if (!IsSupportedMembershipType(request.MembershipType))
            {
                errors.Add("membershipType must be Single, Family, or Couple.");
            }

            return errors;
        }

        private static bool IsValidEmail(string email)
        {
            var value = email.Trim();

            if (value.Contains(' '))
                return false;

            var atIndex = value.IndexOf('@');

            if (atIndex <= 0 || atIndex != value.LastIndexOf('@'))
                return false;

            var domain = value[(atIndex + 1)..];
            var lastDotIndex = domain.LastIndexOf('.');

            return lastDotIndex > 0 && lastDotIndex < domain.Length - 1 && MailAddress.TryCreate(value, out var address)
                && string.Equals(address.Address, value, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSupportedMembershipType(string membershipType)
        {
            return string.Equals(membershipType, "Single", StringComparison.OrdinalIgnoreCase)
                || string.Equals(membershipType, "Family", StringComparison.OrdinalIgnoreCase)
                || string.Equals(membershipType, "Couple", StringComparison.OrdinalIgnoreCase);
        }
    }
}