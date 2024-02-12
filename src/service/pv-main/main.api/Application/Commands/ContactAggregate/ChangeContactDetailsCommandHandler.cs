using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.ContactAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.ContactAggregate
{
    public class ChangeContactDetailsCommandHandler
        : IRequestHandler<ChangeContactDetailsCommand, bool>
    {
        private readonly IRepositoryInt<SiteContactDetail> _siteContactDetailRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeContactDetailsCommandHandler> _logger;

        public ChangeContactDetailsCommandHandler(
            IRepositoryInt<SiteContactDetail> siteContactDetailRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeContactDetailsCommandHandler> logger)
        {
            _siteContactDetailRepository = siteContactDetailRepository ?? throw new ArgumentNullException(nameof(siteContactDetailRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeContactDetailsCommand message, CancellationToken cancellationToken)
        {
            var siteContactDetailFromRepo = await _siteContactDetailRepository.GetAsync(s => s.Id == message.Id);
            if (siteContactDetailFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate contact detail");
            }

            siteContactDetailFromRepo.ChangeDetails(message.OrganisationType, message.OrganisationName, message.DepartmentName, message.ContactFirstName, message.ContactSurname, message.StreetAddress, message.City, message.State, message.PostCode, message.CountryCode, message.ContactNumber, message.ContactEmail);
            _siteContactDetailRepository.Update(siteContactDetailFromRepo);

            _logger.LogInformation($"----- Contact detail {message.Id} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}