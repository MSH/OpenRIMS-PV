using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.PatientAggregate;

namespace PVIMS.API.Application.Validations
{
    public class PatientsDetailQueryValidator : AbstractValidator<PatientsDetailQuery>
    {
        public PatientsDetailQueryValidator(ILogger<PatientsDetailQueryValidator> logger)
        {
            RuleFor(query => query.FacilityName)
                .Length(0, 100)
                .Matches(@"[-a-zA-Z ']")
                .When(c => !string.IsNullOrEmpty(c.FacilityName))
                .WithMessage("Facility name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");

            RuleFor(query => query.FirstName)
                .Length(0, 30)
                .Matches(@"[-a-zA-Z ']")
                .When(c => !string.IsNullOrEmpty(c.FirstName))
                .WithMessage("First name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");

            RuleFor(query => query.LastName)
                .Length(0, 30)
                .Matches(@"[-a-zA-Z ']")
                .When(c => !string.IsNullOrEmpty(c.LastName))
                .WithMessage("Last name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");

            RuleFor(query => query.CaseNumber)
                .Length(0, 50)
                .Matches(@"[-a-zA-Z0-9 .()]")
                .When(c => !string.IsNullOrEmpty(c.CaseNumber))
                .WithMessage("Case number contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, parentheses)");

            RuleFor(query => query.CustomAttributeValue)
                .Matches(@"[-a-zA-Z0-9 ]")
                .When(c => !string.IsNullOrEmpty(c.CustomAttributeValue))
                .WithMessage("Custom attribute value contains invalid characters (Enter A-Z, a-z, space, hyphen)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
