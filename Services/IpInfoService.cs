using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MSLX.Plugin.IPInfo.Models;

namespace MSLX.Plugin.IPInfo.Services;

public class IpInfoService
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(5) };
    
    // API
    private readonly string[] _ipv4Apis = 
    {
        "https://api.ipify.org",
        "https://ipv4.icanhazip.com",
        "https://api.ip.sb/ip",
        "https://v4.ident.me"
    };

    private string _cachedIpv4 = string.Empty;
    private DateTime _lastIpv4Fetch = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1);

    public async Task<string> GetExternalIpv4Async(bool forceRefresh = false)
    {
        if (!forceRefresh && !string.IsNullOrEmpty(_cachedIpv4) && DateTime.Now - _lastIpv4Fetch < _cacheDuration)
        {
            return _cachedIpv4;
        }

        foreach (var api in _ipv4Apis)
        {
            try
            {
                var response = await HttpClient.GetStringAsync(api);
                var ip = response?.Trim();
                if (!string.IsNullOrEmpty(ip) && IPAddress.TryParse(ip, out var parsedIp) && parsedIp.AddressFamily == AddressFamily.InterNetwork)
                {
                    _cachedIpv4 = ip;
                    _lastIpv4Fetch = DateTime.Now;
                    return _cachedIpv4;
                }
            }
            catch (Exception ex)
            {
                SDK.MSLX.Logger.Debug($"Failed to fetch IPv4 from {api}: {ex.Message}");
            }
        }

        return string.Empty;
    }

    public List<NetworkAdapterDetail> GetAllNetworkAdapters()
    {
        var result = new List<NetworkAdapterDetail>();
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var adapter in interfaces)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                continue;

            var detail = new NetworkAdapterDetail
            {
                Name = adapter.Name,
                Description = adapter.Description,
                Type = adapter.NetworkInterfaceType.ToString(),
                Status = adapter.OperationalStatus.ToString(),
                MacAddress = string.Join(":", adapter.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")))
            };

            var ipProps = adapter.GetIPProperties();
            
            // IPv4
            foreach (var ip in ipProps.UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    detail.Ipv4Addresses.Add(new IpAddressDetail
                    {
                        Address = ip.Address.ToString(),
                        PrefixOrMask = ip.IPv4Mask?.ToString() ?? "",
                        Type = "IPv4"
                    });
                }
                else if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    string v6Type = "Unknown";
                    if (ip.Address.IsIPv6LinkLocal) v6Type = "Link Local";
                    else if (ip.Address.IsIPv6SiteLocal) v6Type = "Site Local";
                    else if (ip.Address.IsIPv6Multicast) v6Type = "Multicast";
                    else if (IsUniqueLocalAddress(ip.Address)) v6Type = "Unique Local";
                    else if (!IPAddress.IsLoopback(ip.Address)) v6Type = "Global Unicast";

                    detail.Ipv6Addresses.Add(new IpAddressDetail
                    {
                        Address = ip.Address.ToString(),
                        PrefixOrMask = ip.PrefixLength.ToString(),
                        Type = v6Type
                    });
                }
            }

            result.Add(detail);
        }

        return result;
    }

    public string GetBestExternalIpv6()
    {
        var adapters = GetAllNetworkAdapters();
        foreach (var adapter in adapters)
        {
            if (adapter.Status != "Up") continue;

            var globalUnicast = adapter.Ipv6Addresses.FirstOrDefault(ip => ip.Type == "Global Unicast");
            if (globalUnicast != null)
            {
                var ipAddr = globalUnicast.Address;
                var scopeIndex = ipAddr.IndexOf('%');
                if (scopeIndex > 0)
                {
                    ipAddr = ipAddr.Substring(0, scopeIndex);
                }
                return ipAddr;
            }
        }
        return string.Empty;
    }

    private bool IsUniqueLocalAddress(IPAddress ip)
    {
        if (ip.AddressFamily != AddressFamily.InterNetworkV6) return false;
        var bytes = ip.GetAddressBytes();
        // ULA: fc00::/7
        return bytes[0] == 0xfc || bytes[0] == 0xfd;
    }
}
