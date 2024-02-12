using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class ReportInstanceResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like report instances returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Filter reports by activity qualified name
        /// </summary>
        [StringLength(50)]
        public string QualifiedName { get; set; } = "";

        /// <summary>
        /// Filter encounters by range
        /// </summary>
        public DateTime SearchFrom { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter encounters by range
        /// </summary>
        public DateTime SearchTo { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Filter reports by generic search term
        /// </summary>
        [StringLength(100)]
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// Provide the ability to filter by active reports only
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType ActiveReportsOnly { get; set; } = YesNoValueType.No;
    }
}
