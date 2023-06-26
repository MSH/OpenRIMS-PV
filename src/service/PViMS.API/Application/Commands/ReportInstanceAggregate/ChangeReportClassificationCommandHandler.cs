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
    public class ChangeReportClassificationCommandHandler
        : IRequestHandler<ChangeReportClassificationCommand, bool>
    {
        private readonly IWorkFlowService _workFlowService;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeReportClassificationCommandHandler> _logger;

        public ChangeReportClassificationCommandHandler(
            IWorkFlowService workFlowService,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeReportClassificationCommandHandler> logger)
        {
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeReportClassificationCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            if (await _workFlowService.ValidateExecutionStatusForCurrentActivityAsync(reportInstanceFromRepo.ContextGuid, "CLASSIFICATIONSET") == false)
            {
                throw new DomainException($"Activity CLASSIFICATIONSET not valid for workflow");
            }

            reportInstanceFromRepo.ChangeClassification(message.ReportClassification);

            _reportInstanceRepository.Update(reportInstanceFromRepo);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Report {reportInstanceFromRepo.Id} classification updated");

            await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, "CLASSIFICATIONSET", $"AUTOMATION: Classification set to {message.ReportClassification}", null, "");

            return true;
        }
    }
}
