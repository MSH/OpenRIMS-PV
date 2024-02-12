using OpenRIMS.PV.Main.Core.ValueTypes;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class DatasetMapping : EntityBase
    {
        public DatasetMapping()
        {
            DatasetMappingValues = new HashSet<DatasetMappingValue>();
            SubMappings = new HashSet<DatasetMappingSub>();
        }

        public string Tag { get; set; }
        public MappingType MappingType { get; set; }
        public string MappingOption { get; set; }
        public int DestinationElementId { get; set; }
        public int? SourceElementId { get; set; }
        public string PropertyPath { get; set; }
        public string Property { get; set; }

        public virtual DatasetCategoryElement DestinationElement { get; set; }
        public virtual DatasetCategoryElement SourceElement { get; set; }

        public virtual ICollection<DatasetMappingValue> DatasetMappingValues { get; set; }
        public virtual ICollection<DatasetMappingSub> SubMappings { get; set; }
    }
}