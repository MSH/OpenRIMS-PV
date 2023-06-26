using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    public class AddFacilityToUserCommandHandler
        : IRequestHandler<AddFacilityToUserCommand, bool>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<AddFacilityToUserCommandHandler> _logger;

        public AddFacilityToUserCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<AddFacilityToUserCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AddFacilityToUserCommand message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(u => u.Id == message.UserId,
                    new string[] { "Facilities.Facility" });
            if(userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            var facilityFromRepo = await _facilityRepository.GetAsync(message.FacilityId, new string[] { "" });
            if (facilityFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate facility {message.FacilityId}");
            }

            var newUserFacility = userFromRepo.AddFacility(facilityFromRepo);

            _userRepository.Update(userFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- User {userFromRepo.Id} facilities updated");

            return true;
        }
    }
}