using System.ComponentModel;

namespace OpenRIMS.PV.Main.Core.ValueTypes
{
    public enum PatientOnStudyCriteria
    {
        [Description("Has Encounter in Date Range")]
        HasEncounterinDateRange = 1,

        [Description("Patient Registered in Facility in Date Range")]
        PatientRegisteredinFacilityinDateRange = 2
    }
}
