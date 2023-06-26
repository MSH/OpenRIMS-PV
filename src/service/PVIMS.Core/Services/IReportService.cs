using System;
using System.Collections.Generic;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Services
{
    public interface IReportService
    {
        ICollection<AdverseEventList> GetAdverseEventItems(DateTime searchFrom, DateTime searchTo, AdverseEventCriteria adverseEventCriteria, AdverseEventStratifyCriteria adverseEventStratifyCriteria);
        ICollection<DrugList> GetPatientsByDrugItems(string searchTerm);
        //ICollection<PatientList> GetPatientListByDrugItems(int conceptId);
    }
}
