using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;
using System;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangePatientDateOfBirthCommandValidator : AbstractValidator<ChangePatientDateOfBirthCommand>
    {
        public ChangePatientDateOfBirthCommandValidator(ILogger<ChangePatientDateOfBirthCommandValidator> logger)
        {
            RuleFor(command => command.DateOfBirth)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Birth Date should be before current date")
                .GreaterThanOrEqualTo(p => DateTime.Now.Date.AddYears(-120))
                .WithMessage("Birth Date should be within the past 120 years");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
