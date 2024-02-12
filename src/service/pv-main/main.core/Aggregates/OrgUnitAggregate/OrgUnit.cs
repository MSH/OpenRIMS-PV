using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class OrgUnit : EntityBase
    {
        protected OrgUnit()
        {
            _children = new List<OrgUnit>();
            _facilities = new List<Facility>();
        }

        public OrgUnit(string name, OrgUnit parent, OrgUnitType orgUnitType) : this()
        {
            Name = name;

            ParentId = parent?.id;
            Parent = parent;

            OrgUnitTypeId = orgUnitType.Id;
            OrgUnitType = orgUnitType;
        }

        public string Name { get; private set; }
        
        public int? ParentId { get; private set; }
        public virtual OrgUnit Parent { get; private set; }

        public int OrgUnitTypeId { get; private set; }
        public virtual OrgUnitType OrgUnitType { get; private set; }

        private List<OrgUnit> _children;
        public IEnumerable<OrgUnit> Children => _children.AsReadOnly();

        private List<Facility> _facilities;
        public IEnumerable<Facility> Facilities => _facilities.AsReadOnly();
    }
}