using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Error
{
    public class DuplicateEntryException : Exception
    {
        public DuplicateEntryException(Exception innerException) : base("Duplicate entity entry", innerException)
        {
        }

        public DuplicateEntryException(string message) : base(message)
        {
        }
    }
}
