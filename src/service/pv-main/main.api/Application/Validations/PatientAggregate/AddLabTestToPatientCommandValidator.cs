using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;
using System;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class AddLabTestToPatientCommandValidator : AbstractValidator<AddLabTestToPatientCommand>
    {
        public AddLabTestToPatientCommandValidator(ILogger<AddLabTestToPatientCommandValidator> logger)
        {
            RuleFor(command => command.LabTest)
                .NotEmpty()
                .Length(1, 50);

            RuleFor(command => command.TestResultValue)
                .Length(0, 20)
                .Matches(@"[-a-zA-Z0-9 .]")
                .When(c => !string.IsNullOrEmpty(c.TestResultValue))
                .WithMessage("Patient identifier contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");

            RuleFor(command => command.ReferenceLower)
                .Length(0, 20)
                .Matches(@"[-a-zA-Z0-9 .]")
                .When(c => !string.IsNullOrEmpty(c.ReferenceLower))
                .WithMessage("Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");

            RuleFor(command => command.ReferenceUpper)
                .Length(0, 20)
                .Matches(@"[-a-zA-Z0-9 .]")
                .When(c => !string.IsNullOrEmpty(c.ReferenceLower))
                .WithMessage("Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");

            RuleFor(command => command.TestDate.Date)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Test Date should be before current date")
                .GreaterThanOrEqualTo(p => DateTime.Now.Date.AddYears(-10))
                .WithMessage("Test Date should be within the past 10 years");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
