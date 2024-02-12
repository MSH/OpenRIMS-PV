using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models
{
    public class ContactForUpdateDto
    {
        /// <summary>
        /// The type of organisation
        /// </summary>
        [ValidEnumValue]
        public OrganisationType OrganisationType { get; set; }

        /// <summary>
        /// The name of the organisation
        /// </summary>
        public string OrganisationName { get; set; }

        /// <summary>
        /// The name of the department
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// The first name of the contact at the organisation
        /// </summary>
        public string ContactFirstName { get; set; }

        /// <summary>
        /// The last name of the contact at the organisation
        /// </summary>
        public string ContactLastName { get; set; }

        /// <summary>
        /// The street address of the organisation
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// The city the organisation is located in
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The state the organisation is located in
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The post code the organisation is located in
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// The country code the organisation is located in
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The contact number of the organisation
        /// </summary>
        public string ContactNumber { get; set; }

        /// <summary>
        /// The email address of the organisation
        /// </summary>
        public string ContactEmail { get; set; }
    }
}
