using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    public class ChangeReportInstanceActivityCommandHandler
        : IRequestHandler<ChangeReportInstanceActivityCommand, bool>
    {
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IWorkFlowService _workFlowService;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeReportInstanceActivityCommandHandler> _logger;

        public ChangeReportInstanceActivityCommandHandler(
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IWorkFlowService workFlowService,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeReportInstanceActivityCommandHandler> logger)
        {
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeReportInstanceActivityCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "Activities.ExecutionEvents.ExecutionStatus", "Tasks.CreatedBy", "CreatedBy", "TerminologyMedDra" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            if (await _workFlowService.ValidateExecutionStatusForCurrentActivityAsync(reportInstanceFromRepo.ContextGuid, message.NewExecutionStatus) == false)
            {
                throw new DomainException("Invalid status for activity");
            }

            await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, message.NewExecutionStatus, message.Comments, message.ContextDate, message.ContextCode);

            _logger.LogInformation($"----- Task {message.ReportInstanceId} report instance activity changed");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
