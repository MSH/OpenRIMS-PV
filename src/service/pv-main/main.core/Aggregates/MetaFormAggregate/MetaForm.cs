using System;
using System.Collections.Generic;
using System.Linq;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.SeedWork;

namespace OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate
{
    public class MetaForm 
        : EntityBase, IAggregateRoot
    {
        protected MetaForm()
        {
            Categories = new HashSet<MetaFormCategory>();
        }

        public MetaForm(CohortGroup cohortGroup, string formName, string actionName)
        {
            MetaFormGuid = Guid.NewGuid();
            IsSystem = false;

            CohortGroup = cohortGroup;
            CohortGroupId = cohortGroup.Id;

            FormName = formName;
            ActionName = actionName;
        }

        public int CohortGroupId { get; private set; }
        public virtual CohortGroup CohortGroup { get; private set; }

        public Guid MetaFormGuid { get; private set; }
        public string FormName { get; private set; }
        public string MetaDefinition { get; private set; }
        public bool IsSystem { get; private set; }
        public string ActionName { get; private set; }

        public virtual ICollection<MetaFormCategory> Categories { get; private set; }

        public MetaFormCategory AddCategory(MetaTable metaTable, string categoryName, string help, string icon)
        {
            var newCategory = new MetaFormCategory(metaTable, categoryName, help, icon);
            Categories.Add(newCategory);

            return newCategory;
        }

        public void ChangeCategoryDetails(int categoryId, string categoryName, string help, string icon)
        {
            var category = Categories.SingleOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Unable to locate category {categoryId}");
            }

            category.ChangeDetails(categoryName, help, icon);
        }

        public MetaFormCategoryAttribute AddCategoryAttribute(int categoryId, string attributeName, CustomAttributeConfiguration customAttributeConfiguration, string label, string help)
        {
            var category = Categories.SingleOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Unable to locate category {categoryId}");
            }

            var newCategoryAttribute = new MetaFormCategoryAttribute(attributeName, customAttributeConfiguration, label, help);
            category.Attributes.Add(newCategoryAttribute);

            return newCategoryAttribute;
        }

        public void ChangeCategoryAttributeDetails(int categoryId, int attributeId, string label, string help)
        {
            var category = Categories.SingleOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Unable to locate category {categoryId}");
            }

            var categoryAttribute = category.Attributes.SingleOrDefault(ca => ca.Id == attributeId);
            if (categoryAttribute == null)
            {
                throw new KeyNotFoundException($"Unable to locate category attribute {attributeId}");
            }

            categoryAttribute.ChangeDetails(label, help);
        }

        public void RemoveCategoryAttribute(int categoryId, int attributeId)
        {
            var category = Categories.SingleOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Unable to locate category {categoryId}");
            }

            var categoryAttribute = category.Attributes.SingleOrDefault(ca => ca.Id == attributeId);
            if (categoryAttribute == null)
            {
                throw new KeyNotFoundException($"Unable to locate category attribute {attributeId}");
            }

            category.Attributes.Remove(categoryAttribute);
        }
    }
}