using System.Net.NetworkInformation;

namespace Domain;

public class Door
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public DoorType DoorType { get; set; }

    public IpAddress IpAddress { get; set; }

    public async Task<bool> IsAlive()
    {
        return await PingSuccess(IpAddress.Ip);
    }

    public async Task<bool> OpenDoor()
    {
        return await PingSuccess(IpAddress.Ip);
    }

    private static async Task<bool> PingSuccess(string ip)
    {
        var pingSender = new Ping();
        var reply = await pingSender.SendPingAsync(ip);

        return reply.Status == IPStatus.Success;
    }
}
