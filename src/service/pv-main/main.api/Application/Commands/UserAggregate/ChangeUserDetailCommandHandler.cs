using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.UserAggregate
{
    public class ChangeUserDetailCommandHandler
        : IRequestHandler<ChangeUserDetailCommand, bool>
    {
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeUserDetailCommandHandler> _logger;

        public ChangeUserDetailCommandHandler(
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeUserDetailCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeUserDetailCommand message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(u => u.Id == message.UserId,
                    new string[] { "" });

            if(userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            if (_userRepository.Exists(u => u.UserName == message.UserName && u.Id != message.UserId))
            {
                throw new DomainException("User with same user name already exists");
            }

            userFromRepo.ChangeUserDetails(message.FirstName, message.LastName, message.UserName, message.Email, message.Active, message.AllowDatasetDownload);
            _userRepository.Update(userFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- User {userFromRepo.Id} details updated");

            return true;
        }
    }
}
