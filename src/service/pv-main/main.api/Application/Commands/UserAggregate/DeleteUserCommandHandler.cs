using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.UserAggregate
{
    public class DeleteUserCommandHandler
        : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IRepositoryInt<AuditLog> _auditLogRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<UserFacility> _userFacilityRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            IRepositoryInt<AuditLog> auditLogRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<UserFacility> userFacilityRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userFacilityRepository = userFacilityRepository ?? throw new ArgumentNullException(nameof(userFacilityRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteUserCommand message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(u => u.Id == message.UserId,
                    new string[] { "Facilities" });

            if(userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            if (_auditLogRepository.Exists(a => a.User.Id == message.UserId))
            {
                throw new DomainException("Unable to delete as item is in use");
            }

            var userFacilities = await _userFacilityRepository.ListAsync(c => c.User.Id == message.UserId);
            userFacilities.ToList().ForEach(userFacility => _userFacilityRepository.Delete(userFacility));

            _userRepository.Delete(userFromRepo);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- User {userFromRepo.Id} deleted");

            return true;
        }
    }
}