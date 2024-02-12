using System.Net;

namespace Domain.Exceptions;

public class GarageInteractionFailedException : BusinessRuleException
{
    private const string BaseMessage = "Unable to reserve a parking spot at the garage";
    private new const HttpStatusCode StatusCode = HttpStatusCode.UnprocessableEntity;

    public GarageInteractionFailedException() : base(BaseMessage, StatusCode)
    {
    }

    public GarageInteractionFailedException(string message) : base(BaseMessage + message, StatusCode)
    {
    }

    public GarageInteractionFailedException(string message, Exception inner) : base(BaseMessage + message, StatusCode,
        inner)
    {
    }
}
