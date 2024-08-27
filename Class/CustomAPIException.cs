using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PostDnFhirR4Api.Class
{
    public class CustomAPIException : Exception
    {
        public HttpResponseMessage HttpResponse { get; }

        public CustomAPIException(string message, HttpResponseMessage httpResponse) : base(message)
        {
            HttpResponse = httpResponse;
        }
    }
}
