using System.Net;

namespace Domain.Exceptions;

public class BusinessRuleException : Exception
{
    protected BusinessRuleException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    protected BusinessRuleException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    protected BusinessRuleException(string message, HttpStatusCode statusCode, Exception inner) : base(message, inner)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; set; }
}
