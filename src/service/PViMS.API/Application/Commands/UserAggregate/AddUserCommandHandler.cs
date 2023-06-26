using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using PVIMS.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    public class AddUserCommandHandler
        : IRequestHandler<AddUserCommand, UserIdentifierDto>
    {
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(
            IRepositoryInt<User> userRepository,
            IRepositoryInt<Facility> facilityRepository,
            UserManager<ApplicationUser> userManager,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserIdentifierDto> Handle(AddUserCommand message, CancellationToken cancellationToken)
        {
            if (_userRepository.Exists(u => u.UserName == message.UserName))
            {
                throw new DomainException("User with same user name already exists");
            }

            var identityId = await CreateIdentity(message);
            var facilities = await PrepareFacilityAccess(message.Facilities);

            var newUser = new User(message.FirstName, message.LastName, message.UserName, message.Email, identityId, facilities);

            await _userRepository.SaveAsync(newUser);

            _logger.LogInformation($"----- User {message.UserName} created");

            var mappedUser = _mapper.Map<UserIdentifierDto>(newUser);
            CreateLinks(mappedUser);
            return mappedUser;
        }

        private async Task<Guid> CreateIdentity(AddUserCommand message)
        {
            var newApplicationUser = new ApplicationUser()
            {
                FirstName = message.FirstName,
                LastName = message.LastName,
                UserName = message.UserName,
                Email = message.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(newApplicationUser, message.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new DomainException($"{error.Description} ({error.Code})");
                }
                throw new DomainException($"Unknown error generating user identity");
            }

            IdentityResult roleResult = await _userManager.AddToRolesAsync(newApplicationUser, message.Roles);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new DomainException($"{error.Description} ({error.Code})");
                }
                throw new DomainException($"Unknown error generating user identity roles");
            }

            return newApplicationUser.Id;
        }

        private async Task<List<Facility>> PrepareFacilityAccess(List<string> facilityNames)
        {
            var facilities = new List<Facility>();
            foreach(var facilityName in facilityNames)
            {
                var facilityFromRepo = await _facilityRepository.GetAsync(f => f.FacilityName == facilityName);
                if(facilityFromRepo != null)
                {
                    facilities.Add(facilityFromRepo);
                }
            }
            return facilities;
        }

        private void CreateLinks(UserIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", dto.Id), "self", "GET"));
        }
    }
}
