using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dataset category representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class DatasetCategoryIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the dataset category
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the dataset category
        /// </summary>
        [DataMember]
        public string DatasetCategoryName { get; set; }
    }
}
