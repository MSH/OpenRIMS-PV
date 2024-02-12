namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class FacilityResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like facilities returned in  
        /// Default order attribute is Id  
        /// Other valid options are FacilityName
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "FacilityName";
    }
}
