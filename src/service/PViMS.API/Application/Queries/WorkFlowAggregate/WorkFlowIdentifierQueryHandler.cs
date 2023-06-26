using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
{
    public class WorkFlowIdentifierQueryHandler
        : IRequestHandler<WorkFlowIdentifierQuery, WorkFlowIdentifierDto>
    {
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkFlowIdentifierQueryHandler> _logger;

        public WorkFlowIdentifierQueryHandler(
            IRepositoryInt<WorkFlow> workFlowRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<WorkFlowIdentifierQueryHandler> logger)
        {
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WorkFlowIdentifierDto> Handle(WorkFlowIdentifierQuery message, CancellationToken cancellationToken)
        {
            var workFlowFromRepo = await _workFlowRepository.GetAsync(wf => wf.WorkFlowGuid == message.WorkFlowGuid);

            if(workFlowFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate work flow");
            }

            var mappedWorkFlow = _mapper.Map<WorkFlowIdentifierDto>(workFlowFromRepo);

            return CreateLinks(mappedWorkFlow);
        }

        private WorkFlowIdentifierDto CreateLinks(WorkFlowIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("WorkFlow", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
