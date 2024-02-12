using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangePatientNameCommandValidator : AbstractValidator<ChangePatientNameCommand>
    {
        public ChangePatientNameCommandValidator(ILogger<ChangePatientNameCommandValidator> logger)
        {
            RuleFor(command => command.FirstName)
                .NotEmpty()
                .Length(0, 30)
                .Matches(@"[-a-zA-Z ']")
                .WithMessage("First name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");

            RuleFor(command => command.LastName)
                .NotEmpty()
                .Length(0, 30)
                .Matches(@"[-a-zA-Z ']")
                .WithMessage("Last name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");

            RuleFor(command => command.MiddleName)
                .Length(0, 30)
                .Matches(@"[-a-zA-Z ']")
                .When(c => !string.IsNullOrEmpty(c.MiddleName))
                .WithMessage("Middle name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
