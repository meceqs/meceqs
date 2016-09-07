using System;

namespace Meceqs
{
    public class MeceqsException : Exception
    {
        public MeceqsException() { }
        public MeceqsException(string message) : base(message) { }
        public MeceqsException(string message, Exception inner) : base(message, inner) { }
    }
}