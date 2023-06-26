namespace PVIMS.API.Models.Parameters
{
    public class HolidayResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like holidays  returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";
    }
}
