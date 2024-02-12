using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class LabResultResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like lab results returned in  
        /// Default order attribute is Description  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Description";

        /// <summary>
        /// Provide the ability to filter lab results using a partial or full search term
        /// </summary>
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// Provide the ability to filter by the resources status
        /// </summary>
        [ValidEnumValue]
        public YesNoBothValueType Active { get; set; } = YesNoBothValueType.Yes;
    }
}
