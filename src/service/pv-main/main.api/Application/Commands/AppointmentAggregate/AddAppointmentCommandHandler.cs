using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Commands.AppointmentAggregate
{
    public class AddAppointmentCommandHandler
        : IRequestHandler<AddAppointmentCommand, AppointmentIdentifierDto>
    {
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddAppointmentCommandHandler> _logger;

        public AddAppointmentCommandHandler(
            IRepositoryInt<Appointment> appointmentRepository,
            IRepositoryInt<Patient> patientRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddAppointmentCommandHandler> logger)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentIdentifierDto> Handle(AddAppointmentCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == message.PatientId);
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var appointmentFromRepo = await _appointmentRepository.GetAsync(a => a.PatientId == message.PatientId && a.AppointmentDate == message.AppointmentDate && !a.Archived);
            if (appointmentFromRepo != null)
            {
                throw new DomainException("Patient already has an appointment for this date");
            }

            var newAppointment = new Appointment(message.PatientId, message.AppointmentDate, message.Reason);

            await _appointmentRepository.SaveAsync(newAppointment);

            _logger.LogInformation($"----- Appointment {newAppointment.Id} created");

            var mappedAppointment = _mapper.Map<AppointmentIdentifierDto>(newAppointment);
            CreateLinks(mappedAppointment);

            return mappedAppointment;
        }

        private void CreateLinks(AppointmentIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Appointment", dto.Id), "self", "GET"));
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Appointment", dto.Id), "self", "DELETE"));
        }
    }
}
