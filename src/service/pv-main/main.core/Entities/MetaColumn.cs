using System;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class MetaColumn : EntityBase
    {
        public Guid MetaColumnGuid { get; set; }
        public string ColumnName { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }
        public int ColumnTypeId { get; set; }
        public int TableId { get; set; }
        public string Range { get; set; }

        public virtual MetaTable Table { get; set; }
        public virtual MetaColumnType ColumnType { get; set; }
    }
}
