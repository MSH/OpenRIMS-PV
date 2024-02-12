using OpenRIMS.PV.Main.Core.Exceptions;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate
{
    public class TaskDetail : ValueObject
    {
        public string Source { get; private set; }
        public string Description { get; private set; }

        public TaskDetail() { }

        public TaskDetail(string source, string description)
        {
            SetSource(source);
            SetDescription(description);
        }

        private void SetSource(string source)
        {
            if (String.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            Regex rgx = new Regex(@"[-a-zA-Z0-9<>/ '.]");
            if (!rgx.IsMatch(source))
            {
                throw new DomainException($"{nameof(source)} has invalid characters");
            }

            this.Source = source;
        }

        private void SetDescription(string description)
        {
            if (String.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            Regex rgx = new Regex(@"[-a-zA-Z0-9<>/ '.]");
            if (!rgx.IsMatch(description))
            {
                throw new DomainException($"{nameof(description)} has invalid characters");
            }

            this.Description = description;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Source;
            yield return Description;
        }
    }

}
