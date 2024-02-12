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
    public class AddMetaFormCommandHandler
        : IRequestHandler<AddMetaFormCommand, MetaFormIdentifierDto>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<MetaForm> _metaFormRepositoryy;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddMetaFormCommandHandler> _logger;

        public AddMetaFormCommandHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<MetaForm> metaFormRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddMetaFormCommandHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _metaFormRepositoryy = metaFormRepository ?? throw new ArgumentNullException(nameof(metaFormRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MetaFormIdentifierDto> Handle(AddMetaFormCommand message, CancellationToken cancellationToken)
        {
            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(c => c.Id == message.CohortGroupId);
            if (cohortGroupFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate cohort group {message.CohortGroupId}");
            }

            var newMetaForm = new MetaForm(cohortGroupFromRepo, message.FormName, message.ActionName);

            await _metaFormRepositoryy.SaveAsync(newMetaForm);

            _logger.LogInformation($"----- Meta form {message.FormName} created");

            var mappedMetaForm = _mapper.Map<MetaFormIdentifierDto>(newMetaForm);

            return CreateLinks(mappedMetaForm);
        }

        private MetaFormIdentifierDto CreateLinks(MetaFormIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("MetaForm", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
