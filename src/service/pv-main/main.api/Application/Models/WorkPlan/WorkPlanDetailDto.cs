using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A work plan representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class WorkPlanDetailDto : WorkPlanIdentifierDto
    {
        /// <summary>
        /// The name of the dataset
        /// </summary>
        [DataMember]
        public string DatasetName { get; set; }
    }
}
