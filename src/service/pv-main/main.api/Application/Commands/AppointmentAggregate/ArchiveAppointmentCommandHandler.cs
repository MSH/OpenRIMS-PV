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

namespace OpenRIMS.PV.Main.API.Application.Commands.AppointmentAggregate
{
    public class ArchiveAppointmentCommandHandler
        : IRequestHandler<ArchiveAppointmentCommand, bool>
    {
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ArchiveAppointmentCommandHandler> _logger;

        public ArchiveAppointmentCommandHandler(
            IRepositoryInt<Appointment> appointmentRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ArchiveAppointmentCommandHandler> logger)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ArchiveAppointmentCommand message, CancellationToken cancellationToken)
        {
            var appointmentFromRepo = await _appointmentRepository.GetAsync(a => a.PatientId == message.PatientId && a.Id == message.AppointmentId);
            if (appointmentFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate appointment");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            appointmentFromRepo.Archive(userFromRepo, message.Reason);
            _appointmentRepository.Update(appointmentFromRepo);

            _logger.LogInformation($"----- Appointment {message.AppointmentId} archived");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
