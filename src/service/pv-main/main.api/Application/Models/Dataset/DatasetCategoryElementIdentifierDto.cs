using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dataset category element representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class DatasetCategoryElementIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the dataset category element
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The dataset element unique identifier
        /// </summary>
        [DataMember]
        public int DatasetElementId { get; set; }

        /// <summary>
        /// The name of the dataset element
        /// </summary>
        [DataMember]
        public string ElementName { get; set; }
    }
}
