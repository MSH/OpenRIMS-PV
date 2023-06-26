using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using System;

namespace PVIMS.API.Application.Validations
{
    public class AddPatientCommandValidator : AbstractValidator<AddPatientCommand>
    {
        public AddPatientCommandValidator(ILogger<AddPatientCommandValidator> logger)
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

            RuleFor(command => command.FacilityName)
                .NotEmpty()
                .Length(0, 100);

            RuleFor(command => command.DateOfBirth)
                .LessThanOrEqualTo(c => DateTime.Now.Date)
                .WithMessage("Birth Date should be before current date")
                .GreaterThanOrEqualTo(c => DateTime.Now.Date.AddYears(-120))
                .WithMessage("Birth Date should be within the past 120 years");

            RuleFor(command => command.StartDate)
                .LessThanOrEqualTo(c => DateTime.Now.Date)
                .WithMessage("Condition Start Date should be before current date")
                .GreaterThanOrEqualTo(c => c.DateOfBirth)
                .WithMessage("Condition Start Date should be should be after Birth Date");

            RuleFor(command => command.OutcomeDate)
                .LessThanOrEqualTo(c => DateTime.Now.Date)
                .When(c => c.OutcomeDate.HasValue)
                .WithMessage("Condition Outcome Date should be before current date")
                .GreaterThanOrEqualTo(c => c.StartDate)
                .When(c => c.OutcomeDate.HasValue)
                .WithMessage("Condition Outcome Date should be should be after Start Date");

            RuleFor(command => command.EncounterDate)
                .LessThanOrEqualTo(c => DateTime.Now.Date)
                .WithMessage("Encounter Date should be before current date")
                .GreaterThanOrEqualTo(c => c.DateOfBirth)
                .WithMessage("Encounter Date should be should be after Birth Date");

            RuleFor(command => command.EnroledDate)
                .NotEmpty()
                .When(c => c.CohortGroupId.HasValue)
                .WithMessage("Cohort Enrollment Date should be specified if cohort selected")
                .LessThanOrEqualTo(c => DateTime.Now.Date)
                .When(c => c.EnroledDate.HasValue && c.CohortGroupId.HasValue)
                .WithMessage("Cohort Enrollment Date should be before current date")
                .GreaterThanOrEqualTo(c => c.DateOfBirth)
                .When(c => c.EnroledDate.HasValue && c.CohortGroupId.HasValue)
                .WithMessage("Cohort Enrollment Date should be should be after Birth Date");

            RuleFor(command => command.CaseNumber)
                .Length(0, 50)
                .Matches(@"[-a-zA-Z0-9 .()]")
                .When(c => !string.IsNullOrEmpty(c.Comments))
                .WithMessage("Case number contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, parentheses)");

            RuleFor(command => command.Comments)
                .Length(0, 100)
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .When(c => !string.IsNullOrEmpty(c.Comments))
                .WithMessage("Comments contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
