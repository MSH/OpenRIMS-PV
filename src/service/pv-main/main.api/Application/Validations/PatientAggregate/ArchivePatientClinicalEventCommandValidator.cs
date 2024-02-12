using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ArchivePatientClinicalEventCommandValidator : AbstractValidator<ArchivePatientClinicalEventCommand>
    {
        public ArchivePatientClinicalEventCommandValidator(ILogger<ArchivePatientClinicalEventCommandValidator> logger)
        {
            RuleFor(command => command.Reason)
                .NotEmpty()
                .Length(1, 200)
                .Matches(@"[-a-zA-Z0-9 .']")
                .WithMessage("Reason contains invalid characters (Enter A-Z, a-z, space, period, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
