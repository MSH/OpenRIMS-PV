﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.UserAggregate;

namespace OpenRIMS.PV.Main.API.Application.Validations
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator(ILogger<AddUserCommandValidator> logger)
        {
            RuleFor(command => command.Email)
                .Length(0, 150)
                .When(c => !string.IsNullOrEmpty(c.Email));

            RuleFor(command => command.Password)
                .NotEmpty()
                .Length(1, 100);

            RuleFor(command => command.ConfirmPassword)
                .NotEmpty()
                .Length(1, 100);

            RuleFor(command => command.UserName)
                .NotEmpty()
                .Length(1, 30)
                .Matches(@"[a-zA-Z0-9 ]")
                .WithMessage("User name contains invalid characters (Enter A-Z, a-z, 0-9, space)");

            RuleFor(command => command.FirstName)
                .NotEmpty()
                .Length(1, 30)
                .Matches(@"[a-zA-Z ]")
                .WithMessage("First name contains invalid characters (Enter A-Z, a-z, space)");

            RuleFor(command => command.LastName)
                .NotEmpty()
                .Length(1, 30)
                .Matches(@"[a-zA-Z ]")
                .WithMessage("Last name contains invalid characters (Enter A-Z, a-z, space)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
