using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A representation of a patient's status
    /// </summary>
    [DataContract()]
    public class PatientStatusDto 
    {
        /// <summary>
        /// The effective date of the status
        /// </summary>
        [DataMember]
        public string EffectiveDate { get; set; }

        /// <summary>
        /// Any additional comments to the status
        /// </summary>
        [DataMember]
        public string Comments { get; set; }

        /// <summary>
        /// The status of the patient
        /// </summary>
        [DataMember]
        public string PatientStatus { get; set; }

        /// <summary>
        /// Details of the household creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the household
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }
    }
}
