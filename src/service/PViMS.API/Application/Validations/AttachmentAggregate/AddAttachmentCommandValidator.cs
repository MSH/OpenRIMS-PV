using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.AttachmentAggregate;

namespace PVIMS.API.Application.Validations
{
    public class AddAttachmentCommandValidator : AbstractValidator<AddAttachmentCommand>
    {
        public AddAttachmentCommandValidator(ILogger<AddAttachmentCommandValidator> logger)
        {
            RuleFor(command => command.Description)
                .Length(0, 100)
                .Matches(@"[a-zA-Z0-9 ]")
                .WithMessage("Description contains invalid characters (Enter A-Z, a-z, 0-9, space)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
