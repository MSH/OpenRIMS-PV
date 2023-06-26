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
    public class AddCommentToReportInstanceTaskCommandHandler
        : IRequestHandler<AddCommentToReportInstanceTaskCommand, TaskCommentDto>
    {
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCommentToReportInstanceTaskCommandHandler> _logger;

        public AddCommentToReportInstanceTaskCommandHandler(
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddCommentToReportInstanceTaskCommandHandler> logger)
        {
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TaskCommentDto> Handle(AddCommentToReportInstanceTaskCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "Tasks.CreatedBy", "Tasks.Comments.CreatedBy", "CreatedBy", "WorkFlow" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            var reportInstanceTaskComment = reportInstanceFromRepo.AddTaskComment(message.ReportInstanceTaskId, message.Comment);
            _reportInstanceRepository.Update(reportInstanceFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Task comment {message.Comment} created");

            var mappedReportInstanceTaskComment = _mapper.Map<TaskCommentDto>(reportInstanceTaskComment);

            return CreateLinks(mappedReportInstanceTaskComment);
        }

        private TaskCommentDto CreateLinks(TaskCommentDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("ReportInstanceTaskComment", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
