using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.UserAggregate
{
    public class UserDetailQueryHandler
        : IRequestHandler<UserDetailQuery, UserDetailDto>
    {
        private readonly IRepositoryInt<User> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserDetailQueryHandler> _logger;

        public UserDetailQueryHandler(
            IRepositoryInt<User> userRepository,
            UserManager<ApplicationUser> userManager,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<UserDetailQueryHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDetailDto> Handle(UserDetailQuery message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(f => f.Id == message.UserId, new string[] {
                "Facilities.Facility.OrgUnit"
            });

            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            var mappedUser = _mapper.Map<UserDetailDto>(userFromRepo);

            CreateLinks(mappedUser);
            await CustomUserMapAsync(mappedUser);

            return mappedUser;
        }

        private async Task CustomUserMapAsync(UserDetailDto dto)
        {
            var userFromManager = await _userManager.FindByNameAsync(dto.UserName);
            if(userFromManager == null)
            {
                return;
            }
            var userRoles = await _userManager.GetRolesAsync(userFromManager);
            dto.Roles = userRoles.ToList().ToArray();
        }

        private void CreateLinks(UserDetailDto mappedUser)
        {
            mappedUser.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", mappedUser.Id), "self", "GET"));
            mappedUser.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", mappedUser.Id), "self", "DELETE"));
        }
    }
}
