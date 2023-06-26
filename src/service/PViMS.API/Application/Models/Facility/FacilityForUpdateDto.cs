namespace PVIMS.API.Models
{
    public class FacilityForUpdateDto
    {
        /// <summary>
        /// The name of the facility
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// The code of the facility
        /// </summary>
        public string FacilityCode { get; set; }

        /// <summary>
        /// The type of the facility
        /// </summary>
        public string FacilityType { get; set; }

        /// <summary>
        /// The telephone number
        /// </summary>
        public string TelNumber { get; set; }

        /// <summary>
        /// The mobile number
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// The fax number for the facility
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// The organisation unit the facility has been allocated to
        /// </summary>
        public int? OrgUnitId { get; set; }
    }
}
