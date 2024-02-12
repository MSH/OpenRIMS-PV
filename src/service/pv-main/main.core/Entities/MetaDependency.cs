using System;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class MetaDependency : EntityBase
    {
        public Guid MetaDependencyGuid { get; set; }
        public string ParentColumnName { get; set; }
        public string ReferenceColumnName { get; set; }
        public int ParentTableId { get; set; }
        public int ReferenceTableId { get; set; }

        public virtual MetaTable ParentTable { get; set; }
        public virtual MetaTable ReferenceTable { get; set; }
    }
}
