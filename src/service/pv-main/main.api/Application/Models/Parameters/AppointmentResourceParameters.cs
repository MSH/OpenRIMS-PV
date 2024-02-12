using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class AppointmentResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like appointments returned in  
        /// Default order attribute is Id  
        /// Other valid options are AppointmentDate
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Filter appointments by facility name
        /// </summary>
        [StringLength(100)]
        public string FacilityName { get; set; } = "";

        /// <summary>
        /// Filter appointments by patient id
        /// </summary>
        public int PatientId { get; set; } = 0;

        /// <summary>
        /// Filter appointments by patient first name
        /// </summary>
        public string FirstName { get; set; } = "";

        /// <summary>
        /// Filter appointments by patient last name
        /// </summary>
        public string LastName { get; set; } = "";

        /// <summary>
        /// Filter appointments by criteria
        /// </summary>
        [ValidEnumValue]
        public EncounterSearchCriteria CriteriaId { get; set; } = EncounterSearchCriteria.AllAppointments;

        /// <summary>
        /// Filter appointments by range
        /// </summary>
        public DateTime SearchFrom { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter appointments by range
        /// </summary>
        public DateTime SearchTo { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Filter appointments by custom attribute
        /// </summary>
        public int CustomAttributeId { get; set; } = 0;

        /// <summary>
        /// Filter appointments by custom attribute value
        /// </summary>
        public string CustomAttributeValue { get; set; } = "";
    }
}
