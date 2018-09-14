using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CompanyASP.NET.Helper
{
    public class RepositoryException<T> : Exception
    {
        public T Type { get; set; }
        public RepositoryException(T type, string message = null, Exception innerException = null) : base(message, innerException)
        {
            Type = type;
        }

        private RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
