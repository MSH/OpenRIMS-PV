using System;

namespace PVIMS.Core.Entities
{
    public class PostDeployment : EntityBase
    {
        public Guid ScriptGuid { get; set; }
        public string ScriptFileName { get; set; }
        public string ScriptDescription { get; set; }
        public DateTime? RunDate { get; set; }
        public int? StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public int RunRank { get; set; }
    }
}
