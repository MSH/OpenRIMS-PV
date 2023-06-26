using PVIMS.Core.Aggregates.DatasetAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities
{
    public class DatasetCategory : EntityBase
    {
        public DatasetCategory()
        {
            Acute = false;
            Chronic = false;
            Public = false;
            System = false;

            DatasetCategoryElements = new HashSet<DatasetCategoryElement>();
            WorkPlanCareEventDatasetCategories = new HashSet<WorkPlanCareEventDatasetCategory>();
            DatasetCategoryConditions = new HashSet<DatasetCategoryCondition>();
        }

        public string DatasetCategoryName { get; set; }
        public short CategoryOrder { get; set; }
        public int DatasetId { get; set; }
        public string Uid { get; set; }
        public bool System { get; set; }
        public bool Acute { get; set; }
        public bool Chronic { get; set; }
        public bool Public { get; set; }
        public string FriendlyName { get; set; }
        public string Help { get; set; }

        public virtual Dataset Dataset { get; set; }

        public virtual ICollection<DatasetCategoryElement> DatasetCategoryElements { get; set; }
        public virtual ICollection<WorkPlanCareEventDatasetCategory> WorkPlanCareEventDatasetCategories { get; set; }
        public virtual ICollection<DatasetCategoryCondition> DatasetCategoryConditions { get; set; }

        public void AddElement(DatasetElement datasetElement)
        {
            if (datasetElement == null) { throw new ArgumentNullException(nameof(datasetElement)); };

            if (DatasetCategoryElements.Any(dce => dce.DatasetElement.Id == datasetElement.Id))
            {
                throw new Exception("Element has already been added");
            }

            var datasetCategoryElement = new DatasetCategoryElement()
            {
                DatasetCategory = this,
                Acute = false,
                Chronic = false,
                DatasetElement = datasetElement,
                FieldOrder = 1,
                FriendlyName = string.Empty,
                Help = string.Empty,
                Public = false,
                System = false
            };

            DatasetCategoryElements.Add(datasetCategoryElement);
        }
    }
}
