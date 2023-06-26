using PVIMS.Core.ValueTypes;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class DatasetXmlNode : AuditedEntityBase
    {
        public DatasetXmlNode()
        {
            ChildrenNodes = new HashSet<DatasetXmlNode>();
            NodeAttributes = new HashSet<DatasetXmlAttribute>();
        }

        public string NodeName { get; set; }
        public NodeType NodeType { get; set; }
        public string NodeValue { get; set; }
        public int? ParentNodeId { get; set; }
        public int? DatasetElementId { get; set; }
        public int DatasetXmlId { get; set; }
        public int? DatasetElementSubId { get; set; }

        public virtual DatasetXmlNode ParentNode { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }
        public virtual DatasetElementSub DatasetElementSub { get; set; }
        public virtual DatasetXml DatasetXml { get; set; }

        public virtual ICollection<DatasetXmlNode> ChildrenNodes { get; set; }
        public virtual ICollection<DatasetXmlAttribute> NodeAttributes { get; set; }
   }
}