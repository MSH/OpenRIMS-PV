using OpenRIMS.PV.Main.API.Application.Models.Patient;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    public interface IPatientQueries
    {
        Task<IEnumerable<PatientSearchDto>> SearchPatientsAsync(int currentUserId,
            int? searchFacilityId,
            int? searchPatientId,
            string searchFirstName,
            string searchLastName,
            string caseNumber,
            DateTime? dateOfBirth,
            string customAttributeKey,
            string customAttributeValue);

        Task<IEnumerable<PatientSearchDto>> SearchPatientsByConditionCaseNumberAsync(string caseNumber);

        Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByAnnualAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByQuarterAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByMonthAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByAgeGroupAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByFacilityRegionAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByOutcomeAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByGenderAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByRegimenAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByIsSeriousAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsBySeriousnessAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByClassificationAsync(DateTime searchFrom, DateTime searchTo
            , AgeGroupCriteria ageGroupCriteria
            , string genderId
            , string regimenId
            , int organisationUnitId
            , string outcomeId
            , string isSeriousId
            , string seriousnessId
            , string classificationId);

        Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByEncounterAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByEncounterAsync(DateTime searchFrom, DateTime searchTo, int facilityId);

        Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByFacilityAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByFacilityAsync(DateTime searchFrom, DateTime searchTo, int facilityId);

        Task<IEnumerable<PatientListDto>> GetPatientListByConceptAsync(int conceptId);
    }
}
