using System;
using System.Collections.Generic;
using System.Text;

namespace ECS.Exceptions
{
    class NoComponentException : Exception
    {
        public NoComponentException() : base("The specified component does not exist on the entity") { }
        public NoComponentException(string error) : base(error) { }
    }
}
