using System;
using System.Collections.Generic;
using System.Text;

namespace ECS.Exceptions
{
    class NullEntityException : Exception
    {
        public NullEntityException() : base("The specified entity does not exist") { }
        public NullEntityException(string error) : base(error) { }
    }
}
