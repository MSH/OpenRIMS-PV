using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dataset instance representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetInstanceDetailDto : DatasetInstanceIdentifierDto
    {
        /// <summary>
        /// Details of the encounter creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the encounter
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }

        /// <summary>
        /// Dataset categories linked through it's instance
        /// </summary>
        [DataMember]
        public ICollection<DatasetCategoryViewDto> DatasetCategories { get; set; } = new List<DatasetCategoryViewDto>();
    }
}
