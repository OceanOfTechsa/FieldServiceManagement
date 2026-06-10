using System.Net;
using System.Net.Sockets;

public class LocalIPResolver
{
    /// <summary>
    /// Option 1: Returns all IP addresses assigned to the local machine.
    /// May include IPv4, IPv6, and virtual adapter addresses.
    /// </summary>
    public IEnumerable<string> GetAllAddresses()
    {
        return Dns.GetHostAddresses(Dns.GetHostName())
                  .Select(ip => ip.ToString());
    }

    /// <summary>
    /// Option 2: Returns the first IPv4 address from the host's DNS entry.
    /// Filters by AddressFamily — still may pick up VPN/virtual adapters.
    /// </summary>
    public string? GetPrimaryIPv4()
    {
        return Dns.GetHostAddresses(Dns.GetHostName())
                  .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                  ?.ToString();
    }

    /// <summary>
    /// Option 3: Uses a UDP socket to determine the IP the OS would route
    /// outbound traffic through. Most reliable for finding the "real" LAN IP.
    /// No actual connection is made.
    /// </summary>
    public string? GetRoutedIPv4()
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect("8.8.8.8", 65530);
            return (socket.LocalEndPoint as IPEndPoint)?.Address.ToString();
        }
        catch
        {
            return null;
        }
    }
}