using System;

namespace PVIMS.Core.Exceptions
{
    public class AssertionFailedException : Exception
    {
        public AssertionFailedException(string message)
            : base("Assertion Failed: " + message)
        { }
    }
}
