using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models.Parameters
{
    public class MeddraTermResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like meddra terms returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "MedDraTerm";

        /// <summary>
        /// Filter meddra terms by term type
        /// </summary>
        [StringLength(4)]
        public string TermType { get; set; } = "";

        /// <summary>
        /// Filter meddra terms using parent search term
        /// </summary>
        [StringLength(100)]
        public string ParentSearchTerm { get; set; } = "";

        /// <summary>
        /// Filter meddra terms using search term
        /// </summary>
        [StringLength(100)]
        public string SearchTerm { get; set; } = "";

        /// <summary>
        /// Filter meddra terms using meddra code
        /// </summary>
        [StringLength(10)]
        public string SearchCode { get; set; } = "";
    }
}
