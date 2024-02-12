namespace OpenRIMS.PV.Main.Core.Entities
{
    public class DatasetXmlAttribute : AuditedEntityBase
    {
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public int? DatasetElementId { get; set; }
        public int ParentNodeId { get; set; }

        public virtual DatasetXmlNode ParentNode { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }
    }
}