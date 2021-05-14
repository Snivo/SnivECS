using System;
using System.Collections.Generic;
using System.Text;

namespace ECS.Exceptions
{
    class UnregisteredComponentException : Exception
    {
        public UnregisteredComponentException() : base("The specified component has not been registered") { }
        public UnregisteredComponentException(string error) : base(error) { }
    }
}
