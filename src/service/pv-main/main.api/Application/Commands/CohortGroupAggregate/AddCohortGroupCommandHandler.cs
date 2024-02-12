using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.CohortGroupAggregate
{
    public class AddCohortGroupCommandHandler
        : IRequestHandler<AddCohortGroupCommand, CohortGroupDetailDto>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<Condition> _conditionRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCohortGroupCommandHandler> _logger;

        public AddCohortGroupCommandHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<Condition> conditionRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddCohortGroupCommandHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _conditionRepository = conditionRepository ?? throw new ArgumentNullException(nameof(conditionRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CohortGroupDetailDto> Handle(AddCohortGroupCommand message, CancellationToken cancellationToken)
        {
            var conditionFromRepo = await _conditionRepository.GetAsync(c => c.Description == message.ConditionName);
            if (conditionFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate condition");
            }

            if (_cohortGroupRepository.Exists(cg => cg.CohortName == message.CohortName ||
                cg.CohortCode == message.CohortCode))
            {
                throw new DomainException("Cohort group with same name already exists");
            }

            var newCohortGroup = new CohortGroup(message.CohortName, message.CohortCode, conditionFromRepo, message.StartDate, message.FinishDate);

            await _cohortGroupRepository.SaveAsync(newCohortGroup);

            _logger.LogInformation($"----- Cohort group {message.CohortName} created");

            var mappedCohortGroup = _mapper.Map<CohortGroupDetailDto>(newCohortGroup);

            return CreateLinks(mappedCohortGroup);
        }

        private CohortGroupDetailDto CreateLinks(CohortGroupDetailDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CohortGroup", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
