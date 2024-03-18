using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate
{
    public class FormAttributeType
        : Enumeration
    {
        public static FormAttributeType AlphaNumericTextbox = new FormAttributeType(1, "AlphaNumericTextbox");
        public static FormAttributeType Date = new FormAttributeType(2, "Date");
        public static FormAttributeType DropDownList = new FormAttributeType(3, "DropDownList");
        public static FormAttributeType Listbox = new FormAttributeType(4, "Listbox");
        public static FormAttributeType NumericTextbox = new FormAttributeType(5, "NumericTextbox");
        public static FormAttributeType YesNo = new FormAttributeType(6, "YesNo");
        public static FormAttributeType Label = new FormAttributeType(7, "Label");

        public FormAttributeType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<FormAttributeType> List() =>
            new[] { AlphaNumericTextbox, Date, DropDownList, Listbox, NumericTextbox, YesNo, Label };

        public static FormAttributeType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for FormAttributeType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static FormAttributeType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for FormAttributeType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}