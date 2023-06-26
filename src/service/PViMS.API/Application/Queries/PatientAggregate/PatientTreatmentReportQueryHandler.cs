using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Paging;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientTreatmentReportQueryHandler
        : IRequestHandler<PatientTreatmentReportQuery, LinkedCollectionResourceWrapperDto<PatientsOnTreatmentDto>>
    {
        private readonly IPatientQueries _patientQueries;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientTreatmentReportQueryHandler> _logger;

        public PatientTreatmentReportQueryHandler(
            IPatientQueries patientQueries,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientTreatmentReportQueryHandler> logger)
        {
            _patientQueries = patientQueries ?? throw new ArgumentNullException(nameof(patientQueries));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<PatientsOnTreatmentDto>> Handle(PatientTreatmentReportQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            if (message.PatientOnStudyCriteria == PatientOnStudyCriteria.HasEncounterinDateRange)
            {
                var results = await _patientQueries.GetPatientsOnTreatmentByEncounterAsync(message.SearchFrom, message.SearchTo);
                var pagedResults = PagedCollection<PatientsOnTreatmentDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                foreach (var result in pagedResults)
                {
                    await CustomEncounterMap(result, message.SearchFrom, message.SearchTo, result.FacilityId);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<PatientsOnTreatmentDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForPatientTreatmentReport(wrapper, message.PageNumber, message.PageSize, message.PatientOnStudyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            if (message.PatientOnStudyCriteria == PatientOnStudyCriteria.PatientRegisteredinFacilityinDateRange)
            {
                var results = await _patientQueries.GetPatientsOnTreatmentByEncounterAsync(message.SearchFrom, message.SearchTo);
                var pagedResults = PagedCollection<PatientsOnTreatmentDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                foreach (var result in pagedResults)
                {
                    await CustomFacilityMap(result, message.SearchFrom, message.SearchTo, result.FacilityId);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<PatientsOnTreatmentDto>(pagedResults.TotalCount, pagedResults);
                CreateLinksForPatientTreatmentReport(wrapper, message.PageNumber, message.PageSize, message.PatientOnStudyCriteria,
                    pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private async Task CustomEncounterMap(PatientsOnTreatmentDto dto, DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            var results = await _patientQueries.GetPatientOnTreatmentListByEncounterAsync(searchFrom, searchTo, facilityId);
            dto.Patients = _mapper.Map<List<PatientListDto>>(results);
        }

        private async Task CustomFacilityMap(PatientsOnTreatmentDto dto, DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            var results = await _patientQueries.GetPatientOnTreatmentListByFacilityAsync(searchFrom, searchTo, facilityId);
            dto.Patients = _mapper.Map<List<PatientListDto>>(results);
        }

        private void CreateLinksForPatientTreatmentReport(
            LinkedResourceBaseDto wrapper,
            int pageNumber, int pageSize, PatientOnStudyCriteria patientOnStudyCriteria,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreatePatientTreatmentReportResourceUri(ResourceUriType.Current, pageNumber, pageSize, patientOnStudyCriteria),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientTreatmentReportResourceUri(ResourceUriType.NextPage, pageNumber, pageSize, patientOnStudyCriteria),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientTreatmentReportResourceUri(ResourceUriType.PreviousPage, pageNumber, pageSize, patientOnStudyCriteria),
                       "previousPage", "GET"));
            }
        }
    }
}
