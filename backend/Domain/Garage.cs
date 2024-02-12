namespace Domain;

public class Garage
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<Door> Doors { get; set; }

    public int ParkingSpotsAvailable { get; set; }

    public bool ParkingAvailable()
    {
        return ParkingSpotsAvailable > 0;
    }

    public async Task<bool> GarageHardwareReachableAsync(CancellationToken cancellationToken)
    {
        foreach (var garageDoor in Doors)
            if (!await garageDoor.IsAlive())
                return false;
        return true;
    }

    public async Task OpenEntryDoorAsync()
    {
        await Doors.First(x => x.DoorType == DoorType.Entry).OpenDoor();
    }

    public async Task OpenExitDoorAsync()
    {
        await Doors.First(x => x.DoorType == DoorType.Exit).OpenDoor();
    }
}
