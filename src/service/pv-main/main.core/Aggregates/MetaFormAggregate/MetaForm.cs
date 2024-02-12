using System;
using System.Collections.Generic;
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
    }
}
