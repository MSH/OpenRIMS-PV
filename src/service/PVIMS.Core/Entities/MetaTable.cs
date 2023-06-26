using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class MetaTable : EntityBase
    {
        public MetaTable()
        {
            Columns = new HashSet<MetaColumn>();
            MetaDependencyParentTables = new HashSet<MetaDependency>();
            MetaDependencyReferenceTables = new HashSet<MetaDependency>();
        }

        public Guid MetaTableGuid { get; set; }
        public string TableName { get; set; }
        public string FriendlyName { get; set; }
        public string FriendlyDescription { get; set; }
        public int TableTypeId { get; set; }

        public virtual MetaTableType TableType { get; set; }

        public virtual ICollection<MetaColumn> Columns { get; set; }
        public virtual ICollection<MetaDependency> MetaDependencyParentTables { get; set; }
        public virtual ICollection<MetaDependency> MetaDependencyReferenceTables { get; set; }
    }
}
