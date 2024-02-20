using System;
using System.Collections.Generic;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate
{
    public class MetaFormCategory : EntityBase
    {
        protected MetaFormCategory()
        {
            Attributes = new HashSet<MetaFormCategoryAttribute>();
        }

        public MetaFormCategory(MetaTable metaTable, string categoryName, string help, string icon)
        {
            MetaFormCategoryGuid = Guid.NewGuid();

            MetaTableId = metaTable.Id;
            MetaTable = metaTable;

            CategoryName = categoryName;
            Help = help;
            Icon = icon;
        }

        public int MetaFormId { get; private set; }
        public virtual MetaForm MetaForm { get; private set; }

        public int MetaTableId { get; private set; }
        public virtual MetaTable MetaTable { get; private set; }

        public Guid MetaFormCategoryGuid { get; private set; }
        public string CategoryName { get; private set; }
        public string Help { get; private set; }
        public string Icon { get; private set; }

        public virtual ICollection<MetaFormCategoryAttribute> Attributes { get; private set; }

        public void ChangeDetails(string categoryName, string help, string icon)
        {
            CategoryName = categoryName;
            Help = help;
            Icon = icon;
        }
    }
}
