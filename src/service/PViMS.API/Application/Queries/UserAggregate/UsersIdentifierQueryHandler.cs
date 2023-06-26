using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.UserAggregate
{
    public class UsersIdentifierQueryHandler
        : IRequestHandler<UsersIdentifierQuery, LinkedCollectionResourceWrapperDto<UserIdentifierDto>>
    {
        private readonly IRepositoryInt<User> _userRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersIdentifierQueryHandler> _logger;

        public UsersIdentifierQueryHandler(
            IRepositoryInt<User> userRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<UsersIdentifierQueryHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<UserIdentifierDto>> Handle(UsersIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<User>(message.OrderBy, "asc");

            var predicate = PredicateBuilder.New<User>(true);

            if (!String.IsNullOrWhiteSpace(message.SearchTerm))
            {
                predicate = predicate.And(u => u.UserName.Contains(message.SearchTerm.Trim()) ||
                            u.FirstName.Contains(message.SearchTerm.Trim()) ||
                            u.LastName.Contains(message.SearchTerm.Trim()));
            }


            var pagedUsersFromRepo = await _userRepository.ListAsync(pagingInfo, predicate, orderby, "");
            if (pagedUsersFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedUsers = PagedCollection<UserIdentifierDto>.Create(_mapper.Map<PagedCollection<UserIdentifierDto>>(pagedUsersFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedUsersFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedUsers.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<UserIdentifierDto>(pagedUsersFromRepo.TotalCount, mappedUsers, pagedUsersFromRepo.TotalPages);

                CreateLinksForUsers(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedUsersFromRepo.HasNext, pagedUsersFromRepo.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private void CreateLinks(UserIdentifierDto mappedUser)
        {
            mappedUser.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", mappedUser.Id), "self", "GET"));
            mappedUser.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", mappedUser.Id), "self", "DELETE"));
        }

        private void CreateLinksForUsers(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetUsersByIdentifier", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetUsersByIdentifier", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetUsersByIdentifier", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
