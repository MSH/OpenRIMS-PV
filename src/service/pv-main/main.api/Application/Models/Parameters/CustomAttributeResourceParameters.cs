using OpenRIMS.PV.Main.API.Infrastructure.Attributes;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class CustomAttributeResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like custom attributes returned in  
        /// Default order attribute is AttributeKey  
        /// Other valid options are Id, CustomAttributeConfigurationGuid, ExtendableTypeName  
        /// Attribute must appear in payload to be sortable  
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Filter by extendable type name  
        /// Valid options are 0 = All, 1 = Household, 2 = HouseholdMember
        /// </summary>
        [ValidEnumValue]
        public ExtendableTypeNames ExtendableTypeName { get; set; } = ExtendableTypeNames.All;

        /// <summary>
        /// Filter by attribute type  
        /// Valid options are 0 = All, 1 = None, 2 = Numeric, 3 = String, 4 = Selection, 5 - DateTime, 6 - FirstClassProperty
        /// </summary>
        [ValidEnumValue]
        public CustomAttributeTypes CustomAttributeType { get; set; } = CustomAttributeTypes.All;

        /// <summary>
        /// Filter by searchable values only
        /// Valid options are true, false
        /// </summary>
        public bool? IsSearchable { get; set; }
    }

    public enum ExtendableTypeNames
    {
        All,
        Patient,
        PatientCondition,
        PatientMedication,
        PatientLabTest,
        PatientClinicalEvent
    }

    public enum CustomAttributeTypes
    {
        All,
        None,
        Numeric,
        String,
        Selection,
        DateTime,
        FirstClassProperty
    }
}
