using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using PVIMS.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    public class AddRoleToUserCommandHandler
        : IRequestHandler<AddRoleToUserCommand, bool>
    {
        private readonly IRepositoryInt<User> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AddRoleToUserCommandHandler> _logger;

        public AddRoleToUserCommandHandler(
            IRepositoryInt<User> userRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<AddRoleToUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AddRoleToUserCommand message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(u => u.Id == message.UserId,
                    new string[] { "" });

            if(userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            var userFromManager = await _userManager.FindByNameAsync(userFromRepo.UserName);
            var result = await _userManager.AddToRoleAsync(userFromManager, message.Role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new DomainException($"{error.Description} ({error.Code})");
                }
                throw new DomainException($"Unknown error changing user roles");
            }

            _logger.LogInformation($"----- User {userFromRepo.Id} roles updated");

            return true;
        }
    }
}