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

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    public class AddTaskToReportInstanceCommandHandler
        : IRequestHandler<AddTaskToReportInstanceCommand, TaskDto>
    {
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddTaskToReportInstanceCommandHandler> _logger;

        public AddTaskToReportInstanceCommandHandler(
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddTaskToReportInstanceCommandHandler> logger)
        {
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TaskDto> Handle(AddTaskToReportInstanceCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId, new string[] { "CreatedBy", "WorkFlow" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            var reportInstanceTask = reportInstanceFromRepo.AddTask(message.Source, message.Description, message.TaskType);
            _reportInstanceRepository.Update(reportInstanceFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Task {message.Source} created");

            var mappedReportInstanceTask = _mapper.Map<TaskDto>(reportInstanceTask);

            return CreateLinks(mappedReportInstanceTask);
        }

        private TaskDto CreateLinks(TaskDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("ReportInstanceTask", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
