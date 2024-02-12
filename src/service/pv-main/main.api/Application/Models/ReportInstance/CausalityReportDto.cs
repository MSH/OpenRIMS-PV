using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dto representing the output for a causality report
    /// </summary>
    [DataContract()]
    public class CausalityReportDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique id of the patient with the causality status
        /// </summary>
        [DataMember]
        public string PatientId { get; set; }

        /// <summary>
        /// The first name of the patient with the causality status
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the patient with the causality status
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// The name of the facility
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// The name of the adverse event
        /// </summary>
        [DataMember]
        public string AdverseEvent { get; set; }

        /// <summary>
        /// Is the adverse event serious
        /// </summary>
        [DataMember]
        public string Serious { get; set; }

        /// <summary>
        /// The adverse event date of onset
        /// </summary>
        [DataMember]
        public string OnsetDate { get; set; }

        /// <summary>
        /// Naranjo causality indicator
        /// </summary>
        [DataMember]
        public string NaranjoCausality { get; set; }


        /// <summary>
        /// Who causality indicator
        /// </summary>
        [DataMember]
        public string WhoCausality { get; set; }

        /// <summary>
        /// The medication the causality has been set for
        /// </summary>
        [DataMember]
        public string MedicationIdentifier { get; set; }
    }
}
