using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
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
