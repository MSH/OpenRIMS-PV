using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;

namespace PVIMS.API.Models.Parameters
{
    public class LabTestResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like lab tests returned in  
        /// Default order attribute is Description  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Description";

        /// <summary>
        /// Provide the ability to filter lab tests using a partial or full search term
        /// </summary>
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// Provide the ability to filter by the resources status
        /// </summary>
        [ValidEnumValue]
        public YesNoBothValueType Active { get; set; } = YesNoBothValueType.Yes;
    }
}
