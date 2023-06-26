using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.UserAggregate
{
    public class UserIdentifierQueryHandler
        : IRequestHandler<UserIdentifierQuery, UserIdentifierDto>
    {
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserIdentifierQueryHandler> _logger;

        public UserIdentifierQueryHandler(
            IRepositoryInt<User> userRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<UserIdentifierQueryHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserIdentifierDto> Handle(UserIdentifierQuery message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(f => f.Id == message.UserId);

            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            var mappedUser = _mapper.Map<UserIdentifierDto>(userFromRepo);

            return CreateLinks(mappedUser);
        }

        private UserIdentifierDto CreateLinks(UserIdentifierDto mappedUser)
        {
            mappedUser.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", mappedUser.Id), "self", "GET"));
            mappedUser.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", mappedUser.Id), "self", "DELETE"));

            return mappedUser;
        }
    }
}
