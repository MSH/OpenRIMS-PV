using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A activity history value representation containing audit details of all activities
    /// </summary>
    [DataContract()]
    public class PatientsOnTreatmentDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique id of the facility
        /// </summary>
        [DataMember]
        public int FacilityId { get; set; }

        /// <summary>
        /// The name of the facility
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// Number of patients enrolled at this facility
        /// </summary>
        [DataMember]
        public int PatientCount { get; set; }

        /// <summary>
        /// Number of patients with a non serious event
        /// </summary>
        [DataMember]
        public int PatientWithNonSeriousEventCount { get; set; }

        /// <summary>
        /// Number of patients with a serious event
        /// </summary>
        [DataMember]
        public int PatientWithSeriousEventCount { get; set; }

        /// <summary>
        /// Number of patients with an event
        /// </summary>
        [DataMember]
        public int PatientWithEventCount { get; set; }

        /// <summary>
        /// Percentage of patients with an event
        /// </summary>
        [DataMember]
        public decimal EventPercentage { get; set; }

        /// <summary>
        /// List of patients for selected facility
        /// </summary>
        [DataMember]
        public List<PatientListDto> Patients { get; set; }
    }
}
