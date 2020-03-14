using System;

namespace ConsoleMediaConverter
{
    [Serializable]
    public class CmdArgumentException : ArgumentException
    {
        public string Argument { get; set; }

        public CmdArgumentException() { }
        public CmdArgumentException(string message) : base(message) { }
        public CmdArgumentException(string message, Exception inner) : base(message, inner) { }
        protected CmdArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
