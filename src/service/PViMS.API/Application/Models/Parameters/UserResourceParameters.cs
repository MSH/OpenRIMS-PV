namespace PVIMS.API.Models.Parameters
{
    public class UserResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like lab tests returned in  
        /// Default order attribute is LastName
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "LastName";

        /// <summary>
        /// Provide the ability to filter users using a partial or full search term
        /// </summary>
        public string SearchTerm { get; set; } = "";
    }
}
