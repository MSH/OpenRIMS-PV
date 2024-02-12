using System;

namespace OpenRIMS.PV.Main.Core.Exceptions
{
    public class AssertionFailedException : Exception
    {
        public AssertionFailedException(string message)
            : base("Assertion Failed: " + message)
        { }
    }
}
