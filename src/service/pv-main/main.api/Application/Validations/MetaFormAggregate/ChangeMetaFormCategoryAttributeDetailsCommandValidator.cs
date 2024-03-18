using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangeMetaFormCategoryAttributeDetailsCommandValidator : AbstractValidator<ChangeMetaFormCategoryAttributeDetailsCommand>
    {
        public ChangeMetaFormCategoryAttributeDetailsCommandValidator(ILogger<ChangeMetaFormCategoryAttributeDetailsCommandValidator> logger)
        {
            RuleFor(command => command.Label)
                .NotEmpty()
                .Length(1, 150)
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .WithMessage("Label contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");

            RuleFor(command => command.Help)
                .Length(0, 500)
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .When(c => !string.IsNullOrEmpty(c.Help))
                .WithMessage("Help contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
