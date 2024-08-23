using System;
using System.Net;

using namasdev.Core.Validation;

namespace namasdev.Net.Ftp
{
    [Serializable]
    public class FtpException : Exception
    {
        public FtpException() { }
        public FtpException(string message) : base(message) { }
        public FtpException(string message, Exception inner) : base(message, inner) { }

        public FtpException(FtpStatusCode statusCode, string statusCodeDescripcion)
            : this(statusCodeDescripcion, statusCode, statusCodeDescripcion, null)
        {
        }

        public FtpException(string message, FtpStatusCode statusCode, string statusCodeDescripcion)
            :this(message, statusCode, statusCodeDescripcion, null)
        {
        }

        public FtpException(string message, FtpStatusCode statusCode, string statusCodeDescripcion, Exception inner)
            : this(message, inner)
        {
            Validador.ValidarArgumentRequeridoYThrow(statusCodeDescripcion, nameof(statusCodeDescripcion));

            this.StatusCode = StatusCode;
            this.StatusCodeDescripcion = statusCodeDescripcion;
        }

        protected FtpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public FtpStatusCode StatusCode { get; private set; }
        public string StatusCodeDescripcion { get; private set; }
    }
}
