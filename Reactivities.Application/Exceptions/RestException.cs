using System;
using System.Net;

namespace Reactivities.Application.Exceptions
{
    public class RestException : Exception
    {
        public HttpStatusCode Code { get; set; }

        public object Errors { get; set; }

        public RestException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }
    }
}
