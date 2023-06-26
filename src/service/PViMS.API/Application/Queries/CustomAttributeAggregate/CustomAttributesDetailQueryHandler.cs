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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.CustomAttributeAggregate
{
    public class CustomAttributesDetailQueryHandler
        : IRequestHandler<CustomAttributesDetailQuery, LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _CustomAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomAttributesDetailQueryHandler> _logger;

        public CustomAttributesDetailQueryHandler(
            IRepositoryInt<CustomAttributeConfiguration> CustomAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CustomAttributesDetailQueryHandler> logger)
        {
            _CustomAttributeRepository = CustomAttributeRepository ?? throw new ArgumentNullException(nameof(CustomAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>> Handle(CustomAttributesDetailQuery message, CancellationToken cancellationToken)
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
                var mappedCustomAttributes = PagedCollection<CustomAttributeDetailDto>.Create(_mapper.Map<PagedCollection<CustomAttributeDetailDto>>(pagedCustomAttributesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCustomAttributesFromRepo.TotalCount);

                // Add HATEOAS links to each individual resource
                foreach (var mappedCustomAttribute in mappedCustomAttributes)
                {
                    CreateSelectionValues(mappedCustomAttribute);
                    CreateLinks(mappedCustomAttribute);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>(pagedCustomAttributesFromRepo.TotalCount, mappedCustomAttributes, pagedCustomAttributesFromRepo.TotalPages);

                CreateLinksForCustomAttributes(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedCustomAttributesFromRepo.HasNext, pagedCustomAttributesFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateSelectionValues(CustomAttributeDetailDto dto)
        {
            if (dto.CustomAttributeType != "Selection") { return; };

            dto.SelectionDataItems = _selectionDataItemRepository.List(s => s.AttributeKey == dto.AttributeKey, null, "")
                .Select(ss => new SelectionDataItemDto()
                {
                    SelectionKey = ss.SelectionKey,
                    Value = ss.Value
                })
                .ToList();
        }

        private void CreateLinks(CustomAttributeDetailDto mappedCustomAttribute)
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
                   _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.Current, "GetCustomAttributesByDetail", orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.NextPage, "GetCustomAttributesByDetail", orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateIdResourceUriForWrapper(ResourceUriType.PreviousPage, "GetCustomAttributesByDetail", orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
