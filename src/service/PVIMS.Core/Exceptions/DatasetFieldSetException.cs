using System;

namespace PVIMS.Core.Exceptions
{
    public class DatasetFieldSetException : Exception
    {
        public DatasetFieldSetException(string key, string message)
            :base(message)
        {
            this.Key = key;
        }

        public string Key { get; private set; }
    }
}
