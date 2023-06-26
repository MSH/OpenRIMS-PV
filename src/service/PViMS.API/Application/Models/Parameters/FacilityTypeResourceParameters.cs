namespace PVIMS.API.Models.Parameters
{
    public class FacilityTypeResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like facility types returned in  
        /// Default order attribute is description  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Description";
    }
}
