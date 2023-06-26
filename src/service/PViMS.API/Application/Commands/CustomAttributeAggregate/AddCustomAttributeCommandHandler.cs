using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.CustomAttributeAggregate
{
    public class AddCustomAttributeCommandHandler
        : IRequestHandler<AddCustomAttributeCommand, CustomAttributeIdentifierDto>
    {
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCustomAttributeCommandHandler> _logger;

        public AddCustomAttributeCommandHandler(
            ICustomAttributeService customAttributeService,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddCustomAttributeCommandHandler> logger)
        {
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CustomAttributeIdentifierDto> Handle(AddCustomAttributeCommand message, CancellationToken cancellationToken)
        {
            var detail = new CustomAttributeConfigDetail()
            {
                AttributeDetail = message.AttributeDetail,
                AttributeName = message.AttributeKey,
                Category = message.Category,
                EntityName = message.ExtendableTypeName,
                CustomAttributeType = message.CustomAttributeType,
                Required = message.IsRequired,
                Searchable = message.IsSearchable,
                NumericMaxValue = message.MaxValue,
                NumericMinValue = message.MinValue,
                FutureDateOnly = message.FutureDateOnly.HasValue ? message.FutureDateOnly.Value : false,
                PastDateOnly = message.PastDateOnly.HasValue ? message.PastDateOnly.Value : false,
                StringMaxLength = message.MaxLength
            };

            await _customAttributeService.AddCustomAttributeAsync(detail);

            _logger.LogInformation($"----- Custom Attribute {message.AttributeKey} created");

            var newCustomAttribute = _customAttributeRepository.Get(ca => ca.ExtendableTypeName == message.ExtendableTypeName && ca.AttributeKey == message.AttributeKey);
            if (newCustomAttribute == null)
            {
                throw new KeyNotFoundException("Unable to locate new custom attribute");
            }

            var mappedCustomAttribute = _mapper.Map<CustomAttributeIdentifierDto>(newCustomAttribute);

            CreateLinks(mappedCustomAttribute);

            return mappedCustomAttribute;
        }

        private void CreateLinks(CustomAttributeIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CustomAttribute", dto.Id), "self", "GET"));
        }
    }
}
