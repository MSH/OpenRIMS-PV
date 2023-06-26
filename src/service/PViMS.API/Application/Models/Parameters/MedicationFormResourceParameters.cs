namespace PVIMS.API.Models.Parameters
{
    public class MedicationFormResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like medication forms returned in  
        /// Default order attribute is description  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Description";
    }
}
