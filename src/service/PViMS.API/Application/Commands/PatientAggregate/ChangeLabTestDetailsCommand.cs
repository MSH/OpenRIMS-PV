using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangeLabTestDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientLabTestId { get; private set; }

        [DataMember]
        public string LabTest { get; private set; }

        [DataMember]
        public DateTime TestDate { get; private set; }

        [DataMember]
        public string TestResultCoded { get; private set; }

        [DataMember]
        public string TestResultValue { get; private set; }

        [DataMember]
        public string TestUnit { get; private set; }

        [DataMember]
        public string ReferenceLower { get; private set; }

        [DataMember]
        public string ReferenceUpper { get; private set; }

        [DataMember]
        public IDictionary<int, string> Attributes { get; private set; }

        public ChangeLabTestDetailsCommand()
        {
        }

        public ChangeLabTestDetailsCommand(int patientId, int patientLabTestId, string labTest, DateTime testDate, string testResultCoded, string testResultValue, string testUnit, string referenceLower, string referenceUpper, IDictionary<int, string> attributes): this()
        {
            PatientId = patientId;
            PatientLabTestId = patientLabTestId;
            LabTest = labTest;
            TestDate = testDate;
            TestResultCoded = testResultCoded;
            TestResultValue = testResultValue;
            TestUnit = testUnit;
            ReferenceLower = referenceLower;
            ReferenceUpper = referenceUpper;
            Attributes = attributes;
        }
    }
}
