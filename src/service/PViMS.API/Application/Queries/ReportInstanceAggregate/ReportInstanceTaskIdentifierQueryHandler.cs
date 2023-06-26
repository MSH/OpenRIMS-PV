using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    public class ReportInstanceTaskIdentifierQueryHandler
        : IRequestHandler<ReportInstanceTaskIdentifierQuery, ReportInstanceTaskIdentifierDto>
    {
        private readonly IRepositoryInt<ReportInstanceTask> _reportInstanceTaskRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportInstanceTaskIdentifierQueryHandler> _logger;

        public ReportInstanceTaskIdentifierQueryHandler(
            IRepositoryInt<ReportInstanceTask> reportInstanceTaskRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ReportInstanceTaskIdentifierQueryHandler> logger)
        {
            _reportInstanceTaskRepository = reportInstanceTaskRepository ?? throw new ArgumentNullException(nameof(reportInstanceTaskRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ReportInstanceTaskIdentifierDto> Handle(ReportInstanceTaskIdentifierQuery message, CancellationToken cancellationToken)
        {
            var reportInstanceTaskFromRepo = await _reportInstanceTaskRepository.GetAsync(rit => rit.ReportInstance.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && rit.ReportInstance.Id == message.ReportInstanceId
                    && rit.Id == message.Id);

            if(reportInstanceTaskFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance task");
            }

            var mappedReportInstanceTask = _mapper.Map<ReportInstanceTaskIdentifierDto>(reportInstanceTaskFromRepo);

            return CreateLinks(mappedReportInstanceTask);
        }

        private ReportInstanceTaskIdentifierDto CreateLinks(ReportInstanceTaskIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("ReportInstanceTask", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
