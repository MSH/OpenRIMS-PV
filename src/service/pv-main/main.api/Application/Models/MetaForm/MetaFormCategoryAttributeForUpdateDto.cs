namespace OpenRIMS.PV.Main.API.Models
{
    public class MetaFormCategoryAttributeForUpdateDto
    {
        /// <summary>
        /// The unique name of the attribute, first class or custom attribute
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// The unique id of the custom attribute if one has been selected
        /// </summary>
        public int? CustomAttributeConfigurationId { get; set; }

        /// <summary>
        /// Label associated to the attribute
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Additional help for the attribute
        /// </summary>
        public string Help { get; set; }
    }
}