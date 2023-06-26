using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A holiday representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class HolidayIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the holiday
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The date of the holiday
        /// </summary>
        [DataMember]
        public DateTime HolidayDate { get; set; }

        /// <summary>
        /// The description of the holiday
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}
