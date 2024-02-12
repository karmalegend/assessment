using System.Text.RegularExpressions;

namespace Domain;

public class IpAddress
{
    private string _ip = null!;

    public IpAddress(string ip)
    {
        // https://stackoverflow.com/questions/5284147/validating-ipv4-addresses-with-regexp
        Ip = ValidateIp(ip);
    }

    public IpAddress()
    {
    }

    public string Ip
    {
        get => _ip;
        set => _ip = ValidateIp(value);
    }

    private static string ValidateIp(string ip)
    {
        return Regex.IsMatch(ip, @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$")
            ? ip
            : throw new ArgumentException($"{ip} is not a valid IpAddress (ipv4)", nameof(ip));
    }
}
