using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.UserAggregate
{
    public class ChangeUserFacilitiesCommandHandler
        : IRequestHandler<ChangeUserFacilitiesCommand, bool>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<UserFacility> _userFacilityRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeUserFacilitiesCommandHandler> _logger;

        public ChangeUserFacilitiesCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<UserFacility> userFacilityRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeUserFacilitiesCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userFacilityRepository = userFacilityRepository ?? throw new ArgumentNullException(nameof(userFacilityRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeUserFacilitiesCommand message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(u => u.Id == message.UserId,
                    new string[] { "Facilities" });

            if(userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            await UpdateUserFacilitiesAsync(message.Facilities, userFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- User {userFromRepo.Id} facilities updated");

            return true;
        }

        private async Task UpdateUserFacilitiesAsync(List<string> facilityNames, User userFromRepo)
        {
            var userFacilities = await _userFacilityRepository.ListAsync(uf => uf.User.Id == userFromRepo.Id, null, new string[] { "Facility" });

            var facilitiesToBeRemoved = PrepareFacilitiesToBeRemoved(facilityNames, userFacilities);
            var facilitiesToBeAdded = await PrepareFacilitiesToBeAddedAsync(facilityNames, userFromRepo, userFacilities);

            facilitiesToBeRemoved.ForEach(userFacility => _userFacilityRepository.Delete(userFacility));
            facilitiesToBeAdded.ForEach(userFacility => _userFacilityRepository.Save(userFacility));
        }

        private async Task<List<UserFacility>> PrepareFacilitiesToBeAddedAsync(List<string> facilityNames, User userFromRepo, ICollection<UserFacility> userFacilities)
        {
            var facilitiesToBeAdded = new List<UserFacility>();
            foreach (string facilityName in facilityNames)
            {
                var userFacility = userFacilities.SingleOrDefault(uf => uf.Facility.FacilityName == facilityName);
                if(userFacility == null)
                {
                    var facilityFromRepo = await _facilityRepository.GetAsync(r => r.FacilityName == facilityName);
                    var newUserFacility = new UserFacility(facilityFromRepo, userFromRepo);
                    facilitiesToBeAdded.Add(newUserFacility);
                }
            }
            return facilitiesToBeAdded;
        }

        private List<UserFacility> PrepareFacilitiesToBeRemoved(List<string> facilityNames, ICollection<UserFacility> userFacilities)
        {
            var facilitiesToBeRemoved = new List<UserFacility>();
            foreach (var userFacility in userFacilities)
            {
                if (!facilityNames.Contains(userFacility.Facility.FacilityName))
                {
                    facilitiesToBeRemoved.Add(userFacility);
                }
            }
            return facilitiesToBeRemoved;
        }
    }
}
