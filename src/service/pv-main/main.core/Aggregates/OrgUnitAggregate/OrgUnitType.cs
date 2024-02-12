using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class OrgUnitType : EntityBase
    {
        protected OrgUnitType()
        {
            _children = new List<OrgUnitType>();
            _orgUnits = new List<OrgUnit>();
        }

        public OrgUnitType(string description, OrgUnitType parent) : this()
        {
            Description = description;

            ParentId = parent?.id;
            Parent = parent;
        }

        public string Description { get; private set; }
        
        public int? ParentId { get; private set; }
        public virtual OrgUnitType Parent { get; private set; }

        private List<OrgUnitType> _children;
        public IEnumerable<OrgUnitType> Children => _children.AsReadOnly();

        private List<OrgUnit> _orgUnits;
        public IEnumerable<OrgUnit> OrgUnits => _orgUnits.AsReadOnly();
    }
}