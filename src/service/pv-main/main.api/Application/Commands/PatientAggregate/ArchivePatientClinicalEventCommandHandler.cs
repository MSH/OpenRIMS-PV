using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    public class ArchivePatientClinicalEventCommandHandler
        : IRequestHandler<ArchivePatientClinicalEventCommand, bool>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ArchivePatientClinicalEventCommandHandler> _logger;

        public ArchivePatientClinicalEventCommandHandler(
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ArchivePatientClinicalEventCommandHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ArchivePatientClinicalEventCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { "PatientClinicalEvents" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            patientFromRepo.ArchiveClinicalEvent(message.PatientClinicalEventId, message.Reason, userFromRepo);
            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Clinical Event {message.PatientClinicalEventId} archived");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
