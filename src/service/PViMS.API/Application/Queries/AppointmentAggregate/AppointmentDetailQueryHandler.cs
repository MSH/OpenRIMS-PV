using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    public class AppointmentDetailQueryHandler
        : IRequestHandler<AppointmentDetailQuery, AppointmentDetailDto>
    {
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentDetailQueryHandler> _logger;

        public AppointmentDetailQueryHandler(
            IRepositoryInt<Appointment> appointmentRepository,
            IRepositoryInt<Patient> patientRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AppointmentDetailQueryHandler> logger)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppointmentDetailDto> Handle(AppointmentDetailQuery message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId);
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var appointmentFromRepo = await _appointmentRepository.GetAsync(a => a.PatientId == message.PatientId && a.Id == message.AppointmentId);
            if (appointmentFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate appointment");
            }

            var mappedAppointment = _mapper.Map<AppointmentDetailDto>(appointmentFromRepo);

            return CreateLinks(mappedAppointment);
        }

        private AppointmentDetailDto CreateLinks(AppointmentDetailDto mappedAppointment)
        {
            mappedAppointment.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Appointment", mappedAppointment.Id), "self", "GET"));
            mappedAppointment.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Appointment", mappedAppointment.Id), "self", "DELETE"));

            return mappedAppointment;
        }
    }
}
