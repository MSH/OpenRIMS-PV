using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;

namespace PVIMS.API.Application.Validations
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
