using System;

namespace ConsoleMediaConverter
{
    [Serializable]
    internal class UnknownArgumentException : CmdArgumentException
    {
        public UnknownArgumentException() : this("Unknown argument"){ }
        public UnknownArgumentException(string message) : base(message) { }
        public UnknownArgumentException(string message, Exception inner) : base(message, inner) { }
        protected UnknownArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
