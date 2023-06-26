using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;

namespace PVIMS.API.Models.Parameters
{
    public class ConceptResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like concepts returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "ConceptName";

        /// <summary>
        /// Provide the ability to filter products using a partial or full search term
        /// </summary>
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// Provide the ability to filter by the resources status
        /// </summary>
        [ValidEnumValue]
        public YesNoBothValueType Active { get; set; } = YesNoBothValueType.Yes;
    }
}
