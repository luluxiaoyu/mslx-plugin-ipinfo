using System.Collections.Generic;

namespace MSLX.Plugin.IPInfo.Models;

public class IpSummaryResponse
{
    public string ExternalIpv4 { get; set; } = string.Empty;
    public string ExternalIpv6 { get; set; } = string.Empty;
    public int AdapterCount { get; set; }
}

public class NetworkAdapterDetail
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public List<IpAddressDetail> Ipv4Addresses { get; set; } = new();
    public List<IpAddressDetail> Ipv6Addresses { get; set; } = new();
}

public class IpAddressDetail
{
    public string Address { get; set; } = string.Empty;
    public string PrefixOrMask { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // e.g., "Global Unicast", "Link Local", etc.
}
