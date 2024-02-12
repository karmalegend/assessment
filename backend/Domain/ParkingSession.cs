namespace Domain;

public class ParkingSession
{
    public Guid Id { get; set; }
    public ParkingSessionsState SessionsState { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public User User { get; set; }
    public Guid UserId { get; set; }
    public Garage Garage { get; set; }

    public Guid GarageId { get; set; }
}
