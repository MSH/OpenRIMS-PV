using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ConceptAggregate
{
    public class ChangeConceptDetailsCommandHandler
        : IRequestHandler<ChangeConceptDetailsCommand, bool>
    {
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<MedicationForm> _medicationFormRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeConceptDetailsCommandHandler> _logger;

        public ChangeConceptDetailsCommandHandler(
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<MedicationForm> medicationFormRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeConceptDetailsCommandHandler> logger)
        {
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _medicationFormRepository = medicationFormRepository ?? throw new ArgumentNullException(nameof(medicationFormRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeConceptDetailsCommand message, CancellationToken cancellationToken)
        {
            var conceptFromRepo = await _conceptRepository.GetAsync(c => c.Id == message.Id);
            if (conceptFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate concept");
            }

            var medicationFormFromRepo = await _medicationFormRepository.GetAsync(mf => mf.Description == message.MedicationForm);
            if (medicationFormFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate medication form");
            }

            if (_conceptRepository.Exists(c => c.ConceptName == message.ConceptName && 
                c.Strength == message.Strength &&
                c.MedicationForm.Id == medicationFormFromRepo.Id &&
                c.Id != message.Id))
            {
                throw new DomainException("Item with same name already exists");
            }

            conceptFromRepo.ChangeDetails(message.ConceptName, message.Strength, medicationFormFromRepo);
            if (message.Active)
            {
                conceptFromRepo.MarkAsActive();
            }
            else
            {
                conceptFromRepo.MarkAsInActive();
            } 
            _conceptRepository.Update(conceptFromRepo);

            _logger.LogInformation($"----- Concept {message.ConceptName} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
