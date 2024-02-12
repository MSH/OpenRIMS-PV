using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A activity history value representation containing audit details of all activities
    /// </summary>
    [DataContract()]
    public class ActivityExecutionStatusEventDto
    {
        /// <summary>
        /// The unique identifier of the status event
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique identifier of the adverse event the activity has been performed against
        /// </summary>
        [DataMember]
        public int PatientClinicalEventId { get; set; }

        /// <summary>
        /// The adverse event the activity has been performed against
        /// </summary>
        [DataMember]
        public string AdverseEvent { get; set; }

        /// <summary>
        /// The activity the action has been performed against
        /// </summary>
        [DataMember]
        public string Activity { get; set; }

        /// <summary>
        /// The event that has been executed
        /// </summary>
        [DataMember]
        public string ExecutionEvent { get; set; }

        /// <summary>
        /// Who has executed the event
        /// </summary>
        [DataMember]
        public string ExecutedBy { get; set; }

        /// <summary>
        /// The date and time the event was executed
        /// </summary>
        [DataMember]
        public string ExecutedDate { get; set; }

        /// <summary>
        /// Any additional comments
        /// </summary>
        [DataMember]
        public string Comments { get; set; }

        /// <summary>
        /// The date the E2B extract receipt was confirmed
        /// </summary>
        [DataMember]
        public string ReceiptDate { get; set; }

        /// <summary>
        /// A code for the E2B extract receipt
        /// </summary>
        [DataMember]
        public string ReceiptCode { get; set; }

        /// <summary>
        /// The unique identifier of the patient summary attachment
        /// </summary>
        [DataMember]
        public int PatientSummaryFileId { get; set; }

        /// <summary>
        /// The unique identifier of the patient extract attachment
        /// </summary>
        [DataMember]
        public int PatientExtractFileId { get; set; }

        /// <summary>
        /// The unique identifier of the xml file attachment
        /// </summary>
        [DataMember]
        public int E2bXmlFileId { get; set; }
    }
}
