using System;
using System.Collections.Generic;
using OpenRIMS.PV.Main.Core.Entities.Keyless;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Core.Services
{
    public interface IReportService
    {
        ICollection<AdverseEventList> GetAdverseEventItems(DateTime searchFrom, DateTime searchTo, AdverseEventCriteria adverseEventCriteria, AdverseEventStratifyCriteria adverseEventStratifyCriteria);
        ICollection<DrugList> GetPatientsByDrugItems(string searchTerm);
        //ICollection<PatientList> GetPatientListByDrugItems(int conceptId);
    }
}
