using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;
using System;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class AddMedicationToPatientCommandValidator : AbstractValidator<AddMedicationToPatientCommand>
    {
        public AddMedicationToPatientCommandValidator(ILogger<AddMedicationToPatientCommandValidator> logger)
        {
            RuleFor(command => command.SourceDescription)
                .Length(0, 200) 
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .When(c => !string.IsNullOrEmpty(c.SourceDescription))
                .WithMessage("Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");

            RuleFor(command => command.StartDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Start Date should be before current date")
                .GreaterThanOrEqualTo(p => DateTime.Now.Date.AddYears(-10))
                .WithMessage("Start Date should be within the past 10 years");

            RuleFor(command => command.EndDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Start Date should be before current date")
                .When(p => p.EndDate.HasValue)
                .GreaterThanOrEqualTo(p => p.StartDate)
                .WithMessage("End Date should be after Start Date")
                .When(p => p.EndDate.HasValue);

            RuleFor(command => command.Dose)
                .Length(0, 30)
                .Matches(@"[a-zA-Z0-9./]")
                .When(c => !string.IsNullOrEmpty(c.Dose))
                .WithMessage("Dose contains invalid characters (Enter A-Z, a-z, 0-9, period, forward slash)");

            RuleFor(command => command.DoseFrequency)
                .Length(0, 30)
                .Matches(@"[a-zA-Z0-9. ]")
                .When(c => !string.IsNullOrEmpty(c.DoseFrequency))
                .WithMessage("Dose frequency contains invalid characters (Enter A-Z, a-z, 0-9, period, space)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
