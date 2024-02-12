using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    public class ChangeReportMedicationCausalityCommandHandler
        : IRequestHandler<ChangeReportMedicationCausalityCommand, bool>
    {
        private readonly IWorkFlowService _workFlowService;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeReportMedicationCausalityCommandHandler> _logger;

        public ChangeReportMedicationCausalityCommandHandler(
            IWorkFlowService workFlowService,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeReportMedicationCausalityCommandHandler> logger)
        {
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeReportMedicationCausalityCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "Medications" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            var reportInstanceMedicationFromRepo = reportInstanceFromRepo.Medications.SingleOrDefault(m => m.Id == message.ReportInstanceMedicationId);
            if (reportInstanceMedicationFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance medication");
            }

            if (await _workFlowService.ValidateExecutionStatusForCurrentActivityAsync(reportInstanceFromRepo.ContextGuid, "CAUSALITYSET") == false)
            {
                throw new DomainException($"Activity CAUSALITYSET not valid for workflow");
            }

            if (message.CausalityConfigType == CausalityConfigType.NaranjoScale)
            {
                reportInstanceFromRepo.ChangeMedicationNaranjoCausality(message.ReportInstanceMedicationId, message.Causality);
            }
            if (message.CausalityConfigType == CausalityConfigType.WHOScale)
            {
                reportInstanceFromRepo.ChangeMedicationWhoCausality(message.ReportInstanceMedicationId, message.Causality);
            }

            _reportInstanceRepository.Update(reportInstanceFromRepo);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Report {reportInstanceFromRepo.Id} classification updated");

            await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, "CAUSALITYSET", $"AUTOMATION: Causality set for {reportInstanceMedicationFromRepo.MedicationIdentifier} to {message.Causality}", null, "");

            return true;
        }
    }
}
