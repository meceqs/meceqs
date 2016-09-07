using System;

namespace Meceqs.Configuration
{
    public class UnknownMessageException : MeceqsException
    {
        public UnknownMessageException() { }
        public UnknownMessageException(string message) : base(message) { }
        public UnknownMessageException(string message, Exception inner) : base(message, inner) { }
    }
}