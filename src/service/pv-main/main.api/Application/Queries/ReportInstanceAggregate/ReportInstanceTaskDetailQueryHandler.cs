using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate
{
    public class ReportInstanceTaskDetailQueryHandler
        : IRequestHandler<ReportInstanceTaskDetailQuery, TaskDto>
    {
        private readonly IRepositoryInt<ReportInstanceTask> _reportInstanceTaskRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportInstanceTaskDetailQueryHandler> _logger;

        public ReportInstanceTaskDetailQueryHandler(
            IRepositoryInt<ReportInstanceTask> reportInstanceTaskRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<ReportInstanceTaskDetailQueryHandler> logger)
        {
            _reportInstanceTaskRepository = reportInstanceTaskRepository ?? throw new ArgumentNullException(nameof(reportInstanceTaskRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TaskDto> Handle(ReportInstanceTaskDetailQuery message, CancellationToken cancellationToken)
        {
            var reportInstanceTaskFromRepo = await _reportInstanceTaskRepository.GetAsync(rit => rit.ReportInstance.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && rit.ReportInstance.Id == message.ReportInstanceId
                    && rit.Id == message.Id,
                    new string[] { "Comments.CreatedBy" });

            if(reportInstanceTaskFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance task");
            }

            var mappedReportInstanceTask = _mapper.Map<TaskDto>(reportInstanceTaskFromRepo);

            return CreateLinks(mappedReportInstanceTask);
        }

        private TaskDto CreateLinks(TaskDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("ReportInstanceTask", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
