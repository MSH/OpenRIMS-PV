using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An encounter representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class EncounterDetailDto : EncounterIdentifierDto
    {
        /// <summary>
        /// Any additional notes for the encounter
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// The type of the encounter
        /// </summary>
        [DataMember]
        public string EncounterType { get; set; }

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
        /// The patient that the encounter has been associated to
        /// </summary>
        [DataMember]
        public PatientDetailDto Patient { get; set; }

        /// <summary>
        /// Dataset categories linked to the encounter through it's instance
        /// </summary>
        [DataMember]
        public ICollection<DatasetCategoryViewDto> DatasetCategories { get; set; } = new List<DatasetCategoryViewDto>();
    }
}
