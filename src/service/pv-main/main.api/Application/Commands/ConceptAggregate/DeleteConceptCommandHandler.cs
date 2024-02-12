using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ConceptAggregate
{
    public class DeleteConceptCommandHandler
        : IRequestHandler<DeleteConceptCommand, bool>
    {
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteConceptCommandHandler> _logger;

        public DeleteConceptCommandHandler(
            IRepositoryInt<Concept> conceptRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteConceptCommandHandler> logger)
        {
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteConceptCommand message, CancellationToken cancellationToken)
        {
            var conceptFromRepo = await _conceptRepository.GetAsync(c => c.Id == message.Id, new string[] { 
                "ConditionMedications",
                "PatientMedications"
            });
            if (conceptFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate concept");
            }

            if (conceptFromRepo.ConceptIngredients.Count() > 0 || conceptFromRepo.ConditionMedications.Count() > 0 || conceptFromRepo.PatientMedications.Count() > 0 || conceptFromRepo.Products.Count() > 0)
            {
                throw new DomainException("Unable to delete concept as it is currently in use");
            }

            _conceptRepository.Delete(conceptFromRepo);

            _logger.LogInformation($"----- Concept {message.Id} deleted");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
