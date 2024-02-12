using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.CohortGroupAggregate
{
    public class DeleteCohortGroupCommandHandler
        : IRequestHandler<DeleteCohortGroupCommand, bool>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteCohortGroupCommandHandler> _logger;

        public DeleteCohortGroupCommandHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteCohortGroupCommandHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteCohortGroupCommand message, CancellationToken cancellationToken)
        {
            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(cg => cg.Id == message.Id);
            if (cohortGroupFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate cohort group");
            }

            if (_cohortGroupEnrolmentRepository.Exists(cge => cge.CohortGroup.Id == message.Id))
            {
                throw new DomainException("Unable to delete the Cohort Group as it is currently in use");
            }

            _cohortGroupRepository.Delete(cohortGroupFromRepo);

            _logger.LogInformation($"----- Cohort group {message.Id} deleted");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
