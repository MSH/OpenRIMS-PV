using System;

namespace PVIMS.Core.CustomAttributes
{
    public class CustomAttributeException : Exception
    {
        public CustomAttributeException(string format, params object[] args)
            :base(string.Format(format, args))
        { 
        }
    }
}
