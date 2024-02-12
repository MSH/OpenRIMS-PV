using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Aggregates.NotificationAggregate
{
    public class NotificationType
        : Enumeration
    {
        public static NotificationType Informational = new NotificationType(1, "Informational");
        public static NotificationType Warning = new NotificationType(2, "Warning");
        public static NotificationType Error = new NotificationType(3, "Error");

        public NotificationType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<NotificationType> List() =>
            new[] { Informational, Warning, Error };

        public static NotificationType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for NotificationType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static NotificationType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for NotificationType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}