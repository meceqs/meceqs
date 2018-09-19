using System;

namespace Meceqs.Transport
{
    public class UnknownMessageException : MeceqsException
    {
        public UnknownMessageException() { }
        public UnknownMessageException(string message) : base(message) { }
        public UnknownMessageException(string message, Exception inner) : base(message, inner) { }
    }
}
