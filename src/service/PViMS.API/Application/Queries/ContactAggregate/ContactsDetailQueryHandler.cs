using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ContactAggregate;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ContactAggregate
{
    public class ContactsDetailQueryHandler
        : IRequestHandler<ContactsDetailQuery, LinkedCollectionResourceWrapperDto<ContactDetailDto>>
    {
        private readonly IRepositoryInt<SiteContactDetail> _siteContactDetailRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactsDetailQueryHandler> _logger;

        public ContactsDetailQueryHandler(
            IRepositoryInt<SiteContactDetail> siteContactDetailRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ContactsDetailQueryHandler> logger)
        {
            _siteContactDetailRepository = siteContactDetailRepository ?? throw new ArgumentNullException(nameof(siteContactDetailRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<ContactDetailDto>> Handle(ContactsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<SiteContactDetail>(message.OrderBy, "asc");

            var pagedContactsFromRepo = await _siteContactDetailRepository.ListAsync(pagingInfo, null, orderby, "");
            if (pagedContactsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedContacts = PagedCollection<ContactDetailDto>.Create(_mapper.Map<PagedCollection<ContactDetailDto>>(pagedContactsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedContactsFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedContacts.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<ContactDetailDto>(pagedContactsFromRepo.TotalCount, mappedContacts, pagedContactsFromRepo.TotalPages);

                CreateLinksForContacts(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedContactsFromRepo.HasNext, pagedContactsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(ContactDetailDto mappedContact)
        {
            mappedContact.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Contact", mappedContact.Id), "self", "GET"));
        }

        private void CreateLinksForContacts(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetContactsByDetail", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetContactsByDetail", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetContactsByDetail", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
