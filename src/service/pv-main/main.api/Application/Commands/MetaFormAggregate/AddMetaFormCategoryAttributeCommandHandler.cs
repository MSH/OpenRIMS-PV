using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.CohortGroupAggregate
{
    public class AddMetaFormCategoryAttributeCommandHandler
        : IRequestHandler<AddMetaFormCategoryAttributeCommand, MetaFormCategoryAttributeDto>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeConfigurationRepository;
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddMetaFormCategoryAttributeCommandHandler> _logger;

        public AddMetaFormCategoryAttributeCommandHandler(
            IRepositoryInt<CustomAttributeConfiguration> customAttributeConfigurationRepository,
            IRepositoryInt<MetaForm> metaFormRepository,
            IUnitOfWorkInt unitOfWork,
            IMapper mapper,
            ILogger<AddMetaFormCategoryAttributeCommandHandler> logger)
        {
            _customAttributeConfigurationRepository = customAttributeConfigurationRepository ?? throw new ArgumentNullException(nameof(customAttributeConfigurationRepository));
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MetaFormCategoryAttributeDto> Handle(AddMetaFormCategoryAttributeCommand message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(c => c.Id == message.MetaFormId, new string[] { 
                "Categories.Attributes.CustomAttributeConfiguration" 
            });
            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate meta form {message.MetaFormId}");
            }
            CustomAttributeConfiguration customAttributeConfigurationFromRepo = null;
            if (message.CustomAttributeConfigurationId.HasValue)
            {
                customAttributeConfigurationFromRepo = await _customAttributeConfigurationRepository.GetAsync(c => c.Id == message.CustomAttributeConfigurationId);
            }

            var newMetaFormCategoryAttribute = metaFormFromRepo.AddCategoryAttribute(
                message.MetaFormCategoryId,
                message.AttributeName,
                customAttributeConfigurationFromRepo,
                message.Label,
                message.Help);

            _metaFormRepository.Update(metaFormFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Meta form category attribute {message.Label} Created");

            var mappedMetaFormCategoryAttribute = _mapper.Map<MetaFormCategoryAttributeDto>(newMetaFormCategoryAttribute);

            return mappedMetaFormCategoryAttribute;
        }
    }
}
