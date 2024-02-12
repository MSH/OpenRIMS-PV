using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate
{
    public class TaskStatus
        : Enumeration
    {
        public static TaskStatus New = new TaskStatus(1, "New");
        public static TaskStatus Acknowledged = new TaskStatus(2, "Acknowledged");
        public static TaskStatus OnHold = new TaskStatus(3, "On Hold");
        public static TaskStatus Completed = new TaskStatus(4, "Completed");
        public static TaskStatus Cancelled = new TaskStatus(5, "Cancelled");
        public static TaskStatus Done = new TaskStatus(6, "Done");
        public TaskStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<TaskStatus> List() =>
            new[] { New, Acknowledged, OnHold, Completed, Cancelled, Done };

        public static TaskStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for TaskStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TaskStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for TaskStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}