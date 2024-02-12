using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    public class ChangeReportTerminologyCommandHandler
        : IRequestHandler<ChangeReportTerminologyCommand, bool>
    {
        private readonly IWorkFlowService _workFlowService;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeReportTerminologyCommandHandler> _logger;

        public ChangeReportTerminologyCommandHandler(
            IWorkFlowService workFlowService,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeReportTerminologyCommandHandler> logger)
        {
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeReportTerminologyCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            var terminologyFromRepo = _terminologyMeddraRepository.Get(message.TerminologyMedDraId);
            if (terminologyFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate terminology");
            }

            if (await _workFlowService.ValidateExecutionStatusForCurrentActivityAsync(reportInstanceFromRepo.ContextGuid, "MEDDRASET") == false)
            {
                throw new DomainException($"Activity MEDDRASET not valid for workflow");
            }

            reportInstanceFromRepo.ChangeTerminology(terminologyFromRepo);

            _reportInstanceRepository.Update(reportInstanceFromRepo);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Report {reportInstanceFromRepo.Id} terminology updated");

            await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, "MEDDRASET", $"AUTOMATION: MedDRA Term set to {terminologyFromRepo.DisplayName}", null, "");

            return true;
        }
    }
}
