namespace PVIMS.API.Models.Parameters
{
    public class CohortGroupEnrolmentResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like cohort group enrolments returned in  
        /// Default order attribute is EnroledDate  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "EnroledDate";
    }
}
