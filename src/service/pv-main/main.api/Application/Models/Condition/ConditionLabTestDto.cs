using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A representation of the list of lab tests associated to a condition group
    /// </summary>
    [DataContract()]
    public class ConditionLabTestDto
    {
        /// <summary>
        /// The unique id of the condition lab test
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique id of the lab test
        /// </summary>
        [DataMember]
        public int LabTestId { get; set; }

        /// <summary>
        /// The name of the lab test
        /// </summary>
        [DataMember]
        public string LabTestName { get; set; }
    }
}
