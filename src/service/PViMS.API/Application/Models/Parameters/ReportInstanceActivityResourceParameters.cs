using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models.Parameters
{
    public class ReportInstanceActivityResourceParameters : BaseResourceParameters
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
    }
}
