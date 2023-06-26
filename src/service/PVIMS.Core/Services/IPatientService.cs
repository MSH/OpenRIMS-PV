using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.Core.Services
{
    public interface IPatientService
    {
        SeriesValueList[] GetElementValues(long patientId, string elementName, int records);
        Task<SeriesValueListItem> GetCurrentElementValueForPatientAsync(long patientId, string elementName);

        bool isUnique(List<CustomAttributeParameter> parameters, int patientId = 0);
        bool Exists(List<CustomAttributeParameter> parameters);
        Patient GetPatientUsingAttributes(List<CustomAttributeParameter> parameters);
        Task<int> AddPatientAsync(PatientDetailForCreation patientDetail);
        Task UpdatePatientAsync(PatientDetailForUpdate patientDetail);
        Task<int> AddEncounterAsync(Patient patient, EncounterDetail encounterDetail);
    }
}
