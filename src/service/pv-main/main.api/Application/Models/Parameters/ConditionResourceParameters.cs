using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class ConditionResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like conditions returned in  
        /// Default order attribute is Id  
        /// Other valid options are Description
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Provide the ability to filter by the resources status
        /// </summary>
        [ValidEnumValue]
        public YesNoBothValueType Active { get; set; } = YesNoBothValueType.Yes;
    }
}
