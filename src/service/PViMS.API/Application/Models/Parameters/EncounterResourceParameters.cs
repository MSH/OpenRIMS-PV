using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models.Parameters
{
    public class EncounterResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like encounters returned in  
        /// Default order attribute is Id  
        /// Other valid options are EncounterDate
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Filter patients by facility name
        /// </summary>
        [StringLength(100)]
        public string FacilityName { get; set; } = "";

        /// <summary>
        /// Filter encounters by patient id
        /// </summary>
        public int PatientId { get; set; } = 0;

        /// <summary>
        /// Filter encounters by patient first name
        /// </summary>
        public string FirstName { get; set; } = "";

        /// <summary>
        /// Filter encounters by patient last name
        /// </summary>
        public string LastName { get; set; } = "";

        /// <summary>
        /// Filter encounters by criteria
        /// </summary>
        [ValidEnumValue]
        public EncounterSearchCriteria CriteriaId { get; set; } = EncounterSearchCriteria.AllEncounters;

        /// <summary>
        /// Filter encounters by range
        /// </summary>
        public DateTime SearchFrom { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter encounters by range
        /// </summary>
        public DateTime SearchTo { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Filter encounters by custom attribute
        /// </summary>
        public int CustomAttributeId { get; set; } = 0;

        /// <summary>
        /// Filter encounters by custom attribute value
        /// </summary>
        public string CustomAttributeValue { get; set; } = "";
    }
}
