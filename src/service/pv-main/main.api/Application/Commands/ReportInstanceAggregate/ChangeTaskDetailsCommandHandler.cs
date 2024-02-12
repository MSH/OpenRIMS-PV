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
    public class ChangeTaskDetailsCommandHandler
        : IRequestHandler<ChangeTaskDetailsCommand, bool>
    {
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeTaskDetailsCommandHandler> _logger;

        public ChangeTaskDetailsCommandHandler(
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeTaskDetailsCommandHandler> logger)
        {
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeTaskDetailsCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "Tasks" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            reportInstanceFromRepo.ChangeTaskDetails(message.ReportInstanceTaskId, message.Source, message.Description);
            _reportInstanceRepository.Update(reportInstanceFromRepo);

            _logger.LogInformation($"----- Task {message.Source} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
