using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A spontaneous dataset representation DTO
    /// </summary>
    [DataContract()]
    public class DatasetForSpontaneousDto : DatasetDetailDto
    {
        /// <summary>
        /// Dataset categories linked through it's instance
        /// </summary>
        [DataMember]
        public ICollection<DatasetCategoryViewDto> DatasetCategories { get; set; } = new List<DatasetCategoryViewDto>();
    }
}
