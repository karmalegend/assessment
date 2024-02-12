using System.Net;

namespace Domain.Exceptions;

public class UserHasRunningParkingSessionException : BusinessRuleException
{
    private const string BaseMessage = "User already has a running parking session";
    private new const HttpStatusCode StatusCode = HttpStatusCode.UnprocessableEntity;

    public UserHasRunningParkingSessionException() : base(BaseMessage, StatusCode)
    {
    }

    public UserHasRunningParkingSessionException(string message) : base(BaseMessage + message, StatusCode)
    {
    }

    public UserHasRunningParkingSessionException(string message, Exception inner) : base(BaseMessage + message,
        StatusCode, inner)
    {
    }
}
