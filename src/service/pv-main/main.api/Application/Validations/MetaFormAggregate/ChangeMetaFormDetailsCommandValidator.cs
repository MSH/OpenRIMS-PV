using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangeMetaFormDetailsCommandValidator : AbstractValidator<ChangeMetaFormDetailsCommand>
    {
        public ChangeMetaFormDetailsCommandValidator(ILogger<ChangeMetaFormDetailsCommandValidator> logger)
        {
            RuleFor(command => command.FormName)
                .NotEmpty()
                .Length(1, 50)
                .Matches(@"[a-zA-Z0-9 ']")
                .WithMessage("Form name contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");

            RuleFor(command => command.ActionName)
                .NotEmpty()
                .Length(1, 20)
                .Matches(@"[-a-zA-Z0-9 ]")
                .WithMessage("Action name contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
