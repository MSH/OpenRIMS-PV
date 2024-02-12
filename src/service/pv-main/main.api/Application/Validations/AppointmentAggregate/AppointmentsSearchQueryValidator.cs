using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Queries.AppointmentAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class AppointmentsSearchQueryValidator : AbstractValidator<AppointmentsSearchQuery>
    {
        public AppointmentsSearchQueryValidator(ILogger<AppointmentsSearchQueryValidator> logger)
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

            RuleFor(query => query.CustomAttributeValue)
                .Matches(@"[-a-zA-Z0-9 ]")
                .When(c => !string.IsNullOrEmpty(c.CustomAttributeValue))
                .WithMessage("Custom attribute value contains invalid characters (Enter A-Z, a-z, space, hyphen)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
