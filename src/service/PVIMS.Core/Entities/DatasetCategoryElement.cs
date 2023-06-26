using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class DatasetCategoryElement : EntityBase
	{
        public DatasetCategoryElement()
        {
            Acute = false;
            Chronic = false;
            Public = false;
            System = false;

            DatasetCategoryElementConditions = new HashSet<DatasetCategoryElementCondition>();
            SourceMappings = new HashSet<DatasetMapping>();
            DestinationMappings = new HashSet<DatasetMapping>();
        }

        public short FieldOrder { get; set; }
        public int DatasetCategoryId { get; set; }
        public int DatasetElementId { get; set; }
        public bool Acute { get; set; }
        public bool Chronic { get; set; }
        public string Uid { get; set; }
        public bool System { get; set; }
        public bool Public { get; set; }
        public string FriendlyName { get; set; }
        public string Help { get; set; }

		public virtual DatasetCategory DatasetCategory { get; set; }
		public virtual DatasetElement DatasetElement { get; set; }

        public virtual ICollection<DatasetCategoryElementCondition> DatasetCategoryElementConditions { get; set; }
        public virtual ICollection<DatasetMapping> SourceMappings { get; set; }
        public virtual ICollection<DatasetMapping> DestinationMappings { get; set; }
	}
}