﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.CustomAttributeAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class ChangeCustomAttributeDetailsCommandValidator : AbstractValidator<ChangeCustomAttributeDetailsCommand>
    {
        public ChangeCustomAttributeDetailsCommandValidator(ILogger<ChangeCustomAttributeDetailsCommandValidator> logger)
        {
            RuleFor(command => command.AttributeDetail)
                .Length(0, 150)
                .Matches(@"[a-zA-Z]")
                .WithMessage("Attribute detail contains invalid characters (Enter A-Z, a-z)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
