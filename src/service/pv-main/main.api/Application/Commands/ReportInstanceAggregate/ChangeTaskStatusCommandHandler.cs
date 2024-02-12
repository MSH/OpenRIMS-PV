using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    public class ChangeTaskStatusCommandHandler
        : IRequestHandler<ChangeTaskStatusCommand, bool>
    {
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeTaskStatusCommandHandler> _logger;

        public ChangeTaskStatusCommandHandler(
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeTaskStatusCommandHandler> logger)
        {
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeTaskStatusCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "Tasks.CreatedBy", "CreatedBy", "WorkFlow" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            if(message.TaskStatus.Id == Core.Aggregates.ReportInstanceAggregate.TaskStatus.Cancelled.Id)
            {
                reportInstanceFromRepo.ChangeTaskStatusToCancelled(message.ReportInstanceTaskId);
            }
            if (message.TaskStatus.Id == Core.Aggregates.ReportInstanceAggregate.TaskStatus.Completed.Id)
            {
                reportInstanceFromRepo.ChangeTaskStatusToCompleted(message.ReportInstanceTaskId);
            }
            if (message.TaskStatus.Id == Core.Aggregates.ReportInstanceAggregate.TaskStatus.OnHold.Id)
            {
                reportInstanceFromRepo.ChangeTaskStatusToOnHold(message.ReportInstanceTaskId);
            }
            if (message.TaskStatus.Id == Core.Aggregates.ReportInstanceAggregate.TaskStatus.Acknowledged.Id)
            {
                reportInstanceFromRepo.ChangeTaskStatusToAcknowledged(message.ReportInstanceTaskId);
            }
            if (message.TaskStatus.Id == Core.Aggregates.ReportInstanceAggregate.TaskStatus.Done.Id)
            {
                reportInstanceFromRepo.ChangeTaskStatusToDone(message.ReportInstanceTaskId);
            }

            _reportInstanceRepository.Update(reportInstanceFromRepo);

            _logger.LogInformation($"----- Task {reportInstanceFromRepo.Id} status updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
