using System;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class PatientResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like patients returned in  
        /// Default order attribute is Id  
        /// Other valid options are FirstName, Surname, Age
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Filter patients by facility name
        /// </summary>
        public string FacilityName { get; set; } = "";

        /// <summary>
        /// Filter patients by patient id
        /// </summary>
        public int PatientId { get; set; } = 0;

        /// <summary>
        /// Filter patients by patient first name
        /// </summary>
        public string FirstName { get; set; } = "";

        /// <summary>
        /// Filter patients by patient last name
        /// </summary>
        public string LastName { get; set; } = "";

        /// <summary>
        /// Filter patients by patient date of birth
        /// </summary>
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter patients by condition case number
        /// </summary>
        public string CaseNumber { get; set; } = "";

        /// <summary>
        /// Filter patients by custom attribute
        /// </summary>
        public int CustomAttributeId { get; set; } = 0;

        /// <summary>
        /// Filter patients by custom attribute value
        /// </summary>
        public string CustomAttributeValue { get; set; } = "";
    }
}
