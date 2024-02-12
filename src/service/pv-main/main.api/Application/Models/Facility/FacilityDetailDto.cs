using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A facility representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class FacilityDetailDto : FacilityIdentifierDto
    {
        /// <summary>
        /// The unique code of the facility
        /// </summary>
        [DataMember]
        public string FacilityCode { get; set; }

        /// <summary>
        /// The type of facility
        /// </summary>
        [DataMember]
        public string FacilityType { get; set; }

        /// <summary>
        /// The contact number for the facility
        /// </summary>
        [DataMember]
        public string ContactNumber { get; set; }

        /// <summary>
        /// The mobile number for the facility
        /// </summary>
        [DataMember]
        public string MobileNumber { get; set; }

        /// <summary>
        /// The fax number for the facility
        /// </summary>
        [DataMember]
        public string FaxNumber { get; set; }

        /// <summary>
        /// The organisation unit the facility has been allocated to
        /// </summary>
        [DataMember]
        public int? OrgUnitId { get; set; }
    }
}
