using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate
{
    public class TaskType
        : Enumeration
    {
        public static TaskType DataQualityAssurance = new TaskType(1, "Data Quality Assurance");
        public static TaskType FollowUp = new TaskType(2, "Follow Up");

        public TaskType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<TaskType> List() =>
            new[] { DataQualityAssurance, FollowUp };

        public static TaskType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for TaskType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TaskType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for TaskType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}