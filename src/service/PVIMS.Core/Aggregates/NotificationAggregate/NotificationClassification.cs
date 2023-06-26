using PVIMS.Core.Exceptions;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.NotificationAggregate
{
    public class NotificationClassification
        : Enumeration
    {
        public static NotificationClassification NewTask = new NotificationClassification(1, "NewTask");
        public static NotificationClassification CancelledTask = new NotificationClassification(2, "CancelledTask");
        public static NotificationClassification NewTaskComment = new NotificationClassification(3, "NewTaskComment");
        public static NotificationClassification NewActiveReport = new NotificationClassification(4, "NewActiveReport");
        public static NotificationClassification NewSpontaneousReport = new NotificationClassification(5, "NewSpontaneousReport");
        public static NotificationClassification CausalityAndTerminologySet = new NotificationClassification(6, "CausalityAndTerminologySet");
        public static NotificationClassification AttendedToTask = new NotificationClassification(7, "AttendedToTask");
        public static NotificationClassification E2BSubmitted = new NotificationClassification(8, "E2BSubmitted");

        public NotificationClassification(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<NotificationClassification> List() =>
            new[] { NewTask, CancelledTask, NewTaskComment, NewActiveReport, NewSpontaneousReport, CausalityAndTerminologySet, AttendedToTask, E2BSubmitted };

        public static NotificationClassification FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for NotificationClassification: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static NotificationClassification From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for NotificationClassification: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}