using System.Text.Json.Serialization;

namespace Domain;

public class User
{
    public Guid Id { get; set; }

    public string PartnerId { get; set; } // i'm not entirely sure what this does in the given context? this could be
    // for parkbee to identify what partners they belong to but our authentication
    // suggests we're logged in as a partner?
    // but garages don't seem to be linked to a partner?
    // for now we kind of just accept it exists. and limit user querying to the
    // partner id

    public string LicensePlate { get; set; }

    [JsonIgnore] public ICollection<ParkingSession> ParkingSessions { get; set; }

    public bool HasActiveParkingSessions()
    {
        return ParkingSessions.Any(x => x.SessionsState == ParkingSessionsState.InProgress);
    }
}
