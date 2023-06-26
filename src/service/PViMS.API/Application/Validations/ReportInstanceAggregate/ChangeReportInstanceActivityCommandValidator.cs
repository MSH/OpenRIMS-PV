using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.ReportInstanceAggregate;

namespace PVIMS.API.Application.Validations
{
    public class ChangeReportInstanceActivityCommandValidator : AbstractValidator<ChangeReportInstanceActivityCommand>
    {
        public ChangeReportInstanceActivityCommandValidator(ILogger<ChangeReportInstanceActivityCommandValidator> logger)
        {
            RuleFor(command => command.Comments)
                .Length(0, 100)
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .When(c => !string.IsNullOrEmpty(c.Comments))
                .WithMessage("Comments contains invalid characters (Enter A-Z, a-z, 0-9, period, comma, parentheses, space, apostrophe)");

            RuleFor(command => command.ContextCode)
                .Length(0, 20)
                .Matches(@"[-a-zA-Z0-9']")
                .When(c => !string.IsNullOrEmpty(c.ContextCode))
                .WithMessage("Context code contains invalid characters (Enter A-Z, a-z, 0-9, hyphen)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
