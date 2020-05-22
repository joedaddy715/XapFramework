using System;

namespace Xap.Infrastructure.Exceptions {
    public class XapException : Exception {
        public XapException() { }
        public XapException(string message) : base(message) { }
        public XapException(string message, Exception innerException) : base(message, innerException) { }
    }
}
