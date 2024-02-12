using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate;
using System;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangeConditionDetailsCommandValidator : AbstractValidator<ChangeConditionDetailsCommand>
    {
        public ChangeConditionDetailsCommandValidator(ILogger<ChangeConditionDetailsCommandValidator> logger)
        {
            RuleFor(command => command.StartDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Start Date should be before current date")
                .GreaterThanOrEqualTo(p => DateTime.Now.Date.AddYears(-10))
                .WithMessage("Start Date should be within the past 10 years");

            RuleFor(command => command.OutcomeDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Outcome Date should be before current date")
                .When(p => p.OutcomeDate.HasValue)
                .GreaterThanOrEqualTo(p => p.StartDate)
                .WithMessage("End Date should be after Start Date")
                .When(p => p.OutcomeDate.HasValue);

            RuleFor(command => command.Outcome)
                .Length(0, 50)
                .When(c => !string.IsNullOrEmpty(c.Outcome));

            RuleFor(command => command.TreatmentOutcome)
                .Length(0, 50)
                .When(c => !string.IsNullOrEmpty(c.TreatmentOutcome));

            RuleFor(command => command.CaseNumber)
                .Length(0, 50)
                .Matches(@"[-a-zA-Z0-9 .()]")
                .When(c => !string.IsNullOrEmpty(c.Comments))
                .WithMessage("Case number contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, parentheses)");

            RuleFor(command => command.Comments)
                .Length(0, 500)
                .Matches(@"[-a-zA-Z0-9 .']")
                .When(c => !string.IsNullOrEmpty(c.Comments))
                .WithMessage("Comments contains invalid characters (Enter A-Z, a-z, space, period, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
