using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.AppointmentAggregate
{
    public class ChangeAppointmentDetailsCommandHandler
        : IRequestHandler<ChangeAppointmentDetailsCommand, bool>
    {
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeAppointmentDetailsCommandHandler> _logger;

        public ChangeAppointmentDetailsCommandHandler(
            IRepositoryInt<Appointment> appointmentRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeAppointmentDetailsCommandHandler> logger)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeAppointmentDetailsCommand message, CancellationToken cancellationToken)
        {
            var appointmentFromRepo = await _appointmentRepository.GetAsync(a => a.PatientId == message.PatientId && a.Id == message.AppointmentId);
            if (appointmentFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate appointment");
            }

            if (_appointmentRepository.Exists(a => a.PatientId == message.PatientId && a.AppointmentDate == message.AppointmentDate && a.Id != message.AppointmentId && !a.Archived))
            {
                throw new DomainException("Patient already has an appointment for this date");
            }

            appointmentFromRepo.ChangeDetails(message.AppointmentDate, message.Reason, message.Cancelled, message.CancellationReason);
            _appointmentRepository.Update(appointmentFromRepo);

            _logger.LogInformation($"----- Appointment {appointmentFromRepo.Id} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
