using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.FacilityAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class AddFacilityCommandValidator : AbstractValidator<AddFacilityCommand>
    {
        public AddFacilityCommandValidator(ILogger<AddFacilityCommandValidator> logger)
        {
            RuleFor(command => command.FacilityName)
                .NotEmpty()
                .Length(1, 100)
                .Matches(@"[-a-zA-Z0-9. '()]")
                .WithMessage("Facility name contains invalid characters (Enter A-Z, a-z, 0-9, space, period, apostrophe, round brackets)");

            RuleFor(command => command.FacilityCode)
                .NotEmpty()
                .Length(1, 18)
                .Matches(@"[-a-zA-Z0-9]")
                .WithMessage("Facility code contains invalid characters (Enter A-Z, a-z, 0-9)");

            RuleFor(command => command.TelNumber)
                .Length(0, 30)
                .Matches(@"[-a-zA-Z0-9]")
                .When(c => !string.IsNullOrEmpty(c.TelNumber))
                .WithMessage("Telephone number contains invalid characters (Enter A-Z, a-z, 0-9)");

            RuleFor(command => command.MobileNumber)
                .Length(0, 30)
                .Matches(@"[-a-zA-Z0-9]")
                .When(c => !string.IsNullOrEmpty(c.MobileNumber))
                .WithMessage("Mobile number contains invalid characters (Enter A-Z, a-z, 0-9)");

            RuleFor(command => command.FaxNumber)
                .Length(0, 30)
                .Matches(@"[-a-zA-Z0-9]")
                .When(c => !string.IsNullOrEmpty(c.FaxNumber))
                .WithMessage("Fax number contains invalid characters (Enter A-Z, a-z, 0-9)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
