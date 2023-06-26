using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.CustomAttributeAggregate
{
    public class CustomAttributesIdentifierQueryHandler
        : IRequestHandler<CustomAttributesIdentifierQuery, LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _CustomAttributeRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomAttributesIdentifierQueryHandler> _logger;

        public CustomAttributesIdentifierQueryHandler(
            IRepositoryInt<CustomAttributeConfiguration> CustomAttributeRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CustomAttributesIdentifierQueryHandler> logger)
        {
            _CustomAttributeRepository = CustomAttributeRepository ?? throw new ArgumentNullException(nameof(CustomAttributeRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>> Handle(CustomAttributesIdentifierQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<CustomAttributeConfiguration>(message.OrderBy, "asc");

            var predicate = PredicateBuilder.New<CustomAttributeConfiguration>(true);
            if (message.ExtendableTypeName != ExtendableTypeNames.All)
            {
                predicate = predicate.And(f => f.ExtendableTypeName == message.ExtendableTypeName.ToString());
            }
            if (message.CustomAttributeType != CustomAttributeTypes.All)
            {
                predicate = predicate.And(f => f.CustomAttributeType.ToString() == message.CustomAttributeType.ToString());
            }
            if (message.IsSearchable.HasValue)
            {
                predicate = predicate.And(f => f.IsSearchable == message.IsSearchable);
            }

            var pagedCustomAttributesFromRepo = await _CustomAttributeRepository.ListAsync(pagingInfo, predicate, orderby, "");
            if (pagedCustomAttributesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCustomAttributes = PagedCollection<CustomAttributeIdentifierDto>.Create(_mapper.Map<PagedCollection<CustomAttributeIdentifierDto>>(pagedCustomAttributesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCustomAttributesFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                mappedCustomAttributes.ForEach(dto => CreateLinks(dto));

                var wrapper = new LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>(pagedCustomAttributesFromRepo.TotalCount, mappedCustomAttributes, pagedCustomAttributesFromRepo.TotalPages);

                CreateLinksForCustomAttributes(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedCustomAttributesFromRepo.HasNext, pagedCustomAttributesFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinks(CustomAttributeIdentifierDto mappedCustomAttribute)
        {
            mappedCustomAttribute.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CustomAttribute", mappedCustomAttribute.Id), "self", "GET"));
            mappedCustomAttribute.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CustomAttribute", mappedCustomAttribute.Id), "self", "DELETE"));
        }

        private void CreateLinksForCustomAttributes(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetCustomAttributesByIdentifier", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetCustomAttributesByIdentifier", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetCustomAttributesByIdentifier", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
