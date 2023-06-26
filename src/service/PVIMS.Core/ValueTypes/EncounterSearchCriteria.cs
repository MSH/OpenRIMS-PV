using System.ComponentModel;

namespace PVIMS.Core.ValueTypes
{
    public enum EncounterSearchCriteria
    {
        [Description("All encounters")]
        AllEncounters = 1,

        [Description("All appointments")]
        AllAppointments = 2,

        [Description("Appointments with missed encounter")]
        AppointmentsWithMissedEncounter = 3,

        [Description("Appointments with did not arrive status")]
        AppointmentsWithDidNotArriveStatus = 4,

        [Description("Appointments with encounter")]
        AppointmentsWithEncounter = 5,

        [Description("Appointments with encounter")]
        AllActiveAppointments = 6
    }
}
