using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.ReportInstanceAggregate;

namespace PVIMS.API.Application.Validations
{
    public class ReportInstancesFeedbackQueryValidator : AbstractValidator<ReportInstancesFeedbackQuery>
    {
        public ReportInstancesFeedbackQueryValidator(ILogger<ReportInstancesFeedbackQueryValidator> logger)
        {
            RuleFor(command => command.QualifiedName)
                .NotEmpty()
                .Length(1, 50);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
