using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.CohortGroupAggregate;
using System;

namespace PVIMS.API.Application.Validations
{
    public class AddCohortGroupCommandValidator : AbstractValidator<AddCohortGroupCommand>
    {
        public AddCohortGroupCommandValidator(ILogger<AddCohortGroupCommandValidator> logger)
        {
            RuleFor(command => command.CohortName)
                .NotEmpty()
                .Length(1, 50)
                .Matches(@"[a-zA-Z0-9 ']")
                .WithMessage("Cohort name contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");

            RuleFor(command => command.CohortCode)
                .NotEmpty()
                .Length(1, 5)
                .Matches(@"[-a-zA-Z0-9 ]")
                .WithMessage("Cohort code contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");

            RuleFor(command => command.StartDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Start Date should be before current date")
                .GreaterThanOrEqualTo(p => DateTime.Now.Date.AddYears(-1))
                .WithMessage("Start Date should be within the past year");

            RuleFor(command => command.FinishDate)
                .GreaterThanOrEqualTo(p => p.StartDate)
                .WithMessage("Finish Date should be after Start Date")
                .When(p => p.FinishDate.HasValue);

            RuleFor(command => command.ConditionName)
                .NotEmpty()
                .Length(1, 50)
                .Matches(@"[a-zA-Z ']")
                .WithMessage("Conditions contains invalid characters (Enter A-Z, a-z, space, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
