using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;

namespace PVIMS.API.Application.Validations
{
    public class ArchivePatientLabTestCommandValidator : AbstractValidator<ArchivePatientLabTestCommand>
    {
        public ArchivePatientLabTestCommandValidator(ILogger<ArchivePatientLabTestCommandValidator> logger)
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
