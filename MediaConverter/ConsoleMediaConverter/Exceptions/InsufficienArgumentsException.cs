using System;

namespace ConsoleMediaConverter
{
    [Serializable]
    internal class InsufficienArgumentsException : ArgumentException
    {
        public InsufficienArgumentsException() : this("Insufficient amount of arguments") { }
        public InsufficienArgumentsException(string message) : base(message) { }
        public InsufficienArgumentsException(string message, Exception inner) : base(message, inner) { }
        protected InsufficienArgumentsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
