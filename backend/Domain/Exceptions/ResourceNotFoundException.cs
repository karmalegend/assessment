using System.Net;

namespace Domain.Exceptions;

public class ResourceNotFoundExceptionException : BusinessRuleException
{
    private const string BaseMessage = "Could not find the requested resource";
    private new const HttpStatusCode StatusCode = HttpStatusCode.NotFound;

    public ResourceNotFoundExceptionException() : base(BaseMessage, StatusCode)
    {
    }

    public ResourceNotFoundExceptionException(string message) : base(BaseMessage + message, StatusCode)
    {
    }

    public ResourceNotFoundExceptionException(string message, Exception inner) : base(BaseMessage + message, StatusCode,
        inner)
    {
    }
}
