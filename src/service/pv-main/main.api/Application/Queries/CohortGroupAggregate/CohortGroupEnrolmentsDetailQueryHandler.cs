using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using Extensions = OpenRIMS.PV.Main.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Application.Queries.CohortGroupAggregate
{
    public class CohortGroupEnrolmentsDetailQueryHandler
        : IRequestHandler<CohortGroupEnrolmentsDetailQuery, LinkedCollectionResourceWrapperDto<EnrolmentDetailDto>>
    {
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IPatientService _patientService;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<CohortGroupEnrolmentsDetailQueryHandler> _logger;

        public CohortGroupEnrolmentsDetailQueryHandler(
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IRepositoryInt<Patient> patientRepository,
            IPatientService patientService,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<CohortGroupEnrolmentsDetailQueryHandler> logger)
        {
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<EnrolmentDetailDto>> Handle(CohortGroupEnrolmentsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(cg => cg.Id == message.CohortGroupId);
            if (cohortGroupFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate cohort group {message.CohortGroupId}");
            }

            var orderby = Extensions.GetOrderBy<CohortGroupEnrolment>(message.OrderBy, "asc");

            var predicate = PredicateBuilder.New<CohortGroupEnrolment>(true);
            predicate = predicate.And(cge => cge.CohortGroup.Id == message.CohortGroupId);

            var pagedCohortGroupEnrolmentsFromRepo = await _cohortGroupEnrolmentRepository.ListAsync(pagingInfo, predicate, orderby, new string[] { 
                "CohortGroup", 
                "Patient.PatientFacilities.Facility", 
                "Patient.Encounters" });

            if (pagedCohortGroupEnrolmentsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCohortGroupEnrolments = PagedCollection<EnrolmentDetailDto>.Create(_mapper.Map<PagedCollection<EnrolmentDetailDto>>(pagedCohortGroupEnrolmentsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCohortGroupEnrolmentsFromRepo.TotalCount);

                foreach (var mappedCohortGroupEnrolment in mappedCohortGroupEnrolments)
                {
                    await CustomCohortGroupEnrolmentMapAsync(mappedCohortGroupEnrolment);
                    CreateLinks(mappedCohortGroupEnrolment);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<EnrolmentDetailDto>(pagedCohortGroupEnrolmentsFromRepo.TotalCount, mappedCohortGroupEnrolments, pagedCohortGroupEnrolmentsFromRepo.TotalPages);
                return wrapper;
            }

            return null;
        }

        private void CreateLinks(EnrolmentDetailDto mappedCohortGroupEnrolment)
        {
            mappedCohortGroupEnrolment.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDeenrolmentForPatientResourceUri(mappedCohortGroupEnrolment.PatientId, mappedCohortGroupEnrolment.Id), "deenrol", "PUT"));
        }

        private async Task CustomCohortGroupEnrolmentMapAsync(EnrolmentDetailDto dto)
        {
            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == dto.PatientId, new string[] { "PatientClinicalEvents" });
            if (patientFromRepo == null)
            {
                return;
            }

            dto.CurrentWeight = (await _patientService.GetCurrentElementValueForPatientAsync(dto.PatientId, "Weight (kg)"))?.Value;

            var patientEventSummary = patientFromRepo.GetEventSummary();
            dto.NonSeriousEventCount = patientEventSummary.NonSeriesEventCount;
            dto.SeriousEventCount = patientEventSummary.SeriesEventCount;
        }
    }
}
