using System;

namespace ConsoleMediaConverter
{
    [Serializable]
    public class MultipleArgumentException : CmdArgumentException
    {
        public MultipleArgumentException() : this("Multiple same arguments") { }
        public MultipleArgumentException(string message) : base(message) { }
        public MultipleArgumentException(string message, Exception inner) : base(message, inner) { }
        protected MultipleArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
