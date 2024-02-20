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
    public class AddMetaFormCategoryCommandHandler
        : IRequestHandler<AddMetaFormCategoryCommand, MetaFormCategoryDto>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<MetaForm> _metaFormRepository;
        private readonly IRepositoryInt<MetaTable> _metaTableRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddMetaFormCategoryCommandHandler> _logger;

        public AddMetaFormCategoryCommandHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<MetaForm> metaFormRepository,
            IRepositoryInt<MetaTable> metaTableRepository,
            ILinkGeneratorService linkGeneratorService,
            IUnitOfWorkInt unitOfWork,
            IMapper mapper,
            ILogger<AddMetaFormCategoryCommandHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _metaFormRepository = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _metaTableRepository = metaTableRepository ?? throw new ArgumentNullException(nameof(metaTableRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MetaFormCategoryDto> Handle(AddMetaFormCategoryCommand message, CancellationToken cancellationToken)
        {
            var metaFormFromRepo = await _metaFormRepository.GetAsync(c => c.Id == message.MetaFormId);
            if (metaFormFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate meta form {message.MetaFormId}");
            }
            var metaTableFromRepo = await _metaTableRepository.GetAsync(c => c.Id == message.MetaTableId);
            if (metaTableFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate meta table {message.MetaTableId}");
            }

            var newMetaFormCategory = metaFormFromRepo.AddCategory(
                metaTableFromRepo,
                message.CategoryName,
                message.Help,
                message.Icon);

            _metaFormRepository.Update(metaFormFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Meta form category {message.CategoryName} Created");

            var mappedMetaFormCategory = _mapper.Map<MetaFormCategoryDto>(newMetaFormCategory);

            return mappedMetaFormCategory;
        }
    }
}
