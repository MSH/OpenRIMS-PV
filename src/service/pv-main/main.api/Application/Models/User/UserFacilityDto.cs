using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A user facility representation DTO
    /// </summary>
    [DataContract()]
    public class UserFacilityDto
    {
        /// <summary>
        /// The unique Id of the user facility
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique id of the facility
        /// </summary>
        [DataMember]
        public int FacilityId { get; set; }

        /// <summary>
        /// The facility name
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// The organisation unit the facility is associated to
        /// </summary>
        [DataMember]
        public string OrgUnitName { get; set; }
    }
}
