using PVIMS.Core.Exceptions;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.ReportInstanceAggregate
{
    public class ReportClassification
        : Enumeration
    {
        public static ReportClassification AESI = new ReportClassification(1, "AESI");
        public static ReportClassification SAE = new ReportClassification(2, "SAE");
        public static ReportClassification ClinicallySignificant = new ReportClassification(3, "Clinically Significant");
        public static ReportClassification Unclassified = new ReportClassification(4, "Unclassified");
        public ReportClassification(int id, string name)
            : base(id, name)
        {
        }
        public static IEnumerable<ReportClassification> List() =>
            new[] { AESI, SAE, ClinicallySignificant, Unclassified };

        public static ReportClassification FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for ReportClassification: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static ReportClassification From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for ReportClassification: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}