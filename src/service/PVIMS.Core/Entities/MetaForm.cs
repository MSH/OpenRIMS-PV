using System;

namespace PVIMS.Core.Entities
{
    public class MetaForm : EntityBase
    {
        public MetaForm()
        {
            MetaFormGuid = Guid.NewGuid();
            IsSystem = false;
        }

        public Guid MetaFormGuid { get; set; }
        public string FormName { get; set; }
        public string MetaDefinition { get; set; }
        public bool IsSystem { get; set; }
        public string ActionName { get; set; }
    }
}
