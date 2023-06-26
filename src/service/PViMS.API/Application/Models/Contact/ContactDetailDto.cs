using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A contact detail representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class ContactDetailDto : ContactIdentifierDto
    {
        /// <summary>
        /// The type of organisation
        /// </summary>
        [DataMember]
        public string OrganisationType { get; set; }

        /// <summary>
        /// The name of the organisation
        /// </summary>
        [DataMember]
        public string OrganisationName { get; set; }

        /// <summary>
        /// The name of the department
        /// </summary>
        [DataMember]
        public string DepartmentName { get; set; }

        /// <summary>
        /// The street address of the organisation
        /// </summary>
        [DataMember]
        public string StreetAddress { get; set; }

        /// <summary>
        /// The city the organisation is located in
        /// </summary>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        /// The state the organisation is located in
        /// </summary>
        [DataMember]
        public string State { get; set; }

        /// <summary>
        /// The post code the organisation is located in
        /// </summary>
        [DataMember]
        public string PostCode { get; set; }

        /// <summary>
        /// The country code the organisation is located in
        /// </summary>
        [DataMember]
        public string CountryCode { get; set; }

        /// <summary>
        /// The contact number of the organisation
        /// </summary>
        [DataMember]
        public string ContactNumber { get; set; }

        /// <summary>
        /// The email address of the organisation
        /// </summary>
        [DataMember]
        public string ContactEmail { get; set; }
    }
}
