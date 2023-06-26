using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.ReportInstanceAggregate;

namespace PVIMS.API.Application.Validations
{
    public class AddCommentToReportInstanceTaskCommandValidator : AbstractValidator<AddCommentToReportInstanceTaskCommand>
    {
        public AddCommentToReportInstanceTaskCommandValidator(ILogger<AddTaskToReportInstanceCommandValidator> logger)
        {
            RuleFor(command => command.Comment).NotEmpty().Length(1, 500);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
