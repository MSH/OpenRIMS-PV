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
    public class ChangeCohortGroupDetailsCommandHandler
        : IRequestHandler<ChangeCohortGroupDetailsCommand, bool>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<Condition> _conditionRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeCohortGroupDetailsCommandHandler> _logger;

        public ChangeCohortGroupDetailsCommandHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<Condition> conditionRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeCohortGroupDetailsCommandHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _conditionRepository = conditionRepository ?? throw new ArgumentNullException(nameof(conditionRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeCohortGroupDetailsCommand message, CancellationToken cancellationToken)
        {
            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(cg => cg.Id == message.Id);
            if (cohortGroupFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate cohort group");
            }

            var conditionFromRepo = await _conditionRepository.GetAsync(c => c.Description == message.ConditionName);
            if (conditionFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate condition");
            }

            if (_cohortGroupRepository.Exists(l => (l.CohortName == message.CohortName || l.CohortCode == message.CohortCode) && l.Id != message.Id))
            {
                throw new DomainException("Item with same name already exists");
            }

            cohortGroupFromRepo.ChangeDetails(message.CohortName, message.CohortCode, conditionFromRepo, message.StartDate, message.FinishDate);
            _cohortGroupRepository.Update(cohortGroupFromRepo);

            _logger.LogInformation($"----- Cohort group {message.CohortName} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
