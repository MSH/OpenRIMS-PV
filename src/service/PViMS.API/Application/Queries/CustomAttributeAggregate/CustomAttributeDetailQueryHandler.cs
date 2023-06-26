using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.CustomAttributeAggregate
{
    public class CustomAttributeDetailQueryHandler
        : IRequestHandler<CustomAttributeDetailQuery, CustomAttributeDetailDto>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomAttributeDetailQueryHandler> _logger;

        public CustomAttributeDetailQueryHandler(
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CustomAttributeDetailQueryHandler> logger)
        {
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CustomAttributeDetailDto> Handle(CustomAttributeDetailQuery message, CancellationToken cancellationToken)
        {
            var customAttributeFromRepo = await _customAttributeRepository.GetAsync(ca => ca.Id == message.CustomAttributeId, new string[] { "" });

            if (customAttributeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate custom attribute");
            }

            var mappedCustomAttribute = _mapper.Map<CustomAttributeDetailDto>(customAttributeFromRepo);

            CreateSelectionValues(mappedCustomAttribute);
            CreateLinks(mappedCustomAttribute);

            return mappedCustomAttribute;
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

        private void CreateLinks(CustomAttributeDetailDto mappedFacility)
        {
            mappedFacility.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", mappedFacility.Id), "self", "GET"));
            mappedFacility.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", mappedFacility.Id), "self", "DELETE"));
        }
    }
}
