using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangePatientFacilityCommandValidator : AbstractValidator<ChangePatientFacilityCommand>
    {
        public ChangePatientFacilityCommandValidator(ILogger<ChangePatientFacilityCommandValidator> logger)
        {
            RuleFor(command => command.FacilityName)
                .NotEmpty()
                .Length(0, 100);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
