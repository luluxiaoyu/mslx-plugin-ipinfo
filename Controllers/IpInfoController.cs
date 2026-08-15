using Microsoft.AspNetCore.Mvc;
using MSLX.SDK.Models;
using MSLX.Plugin.IPInfo.Models;
using MSLX.Plugin.IPInfo.Services;
using System.Threading.Tasks;

namespace MSLX.Plugin.IPInfo.Controllers;

[ApiController]
[Route("api/plugins/mslx-plugin-ipinfo/ip")]
public class IpInfoController : ControllerBase
{
    private readonly IpInfoService _ipInfoService;

    public IpInfoController(IpInfoService ipInfoService)
    {
        _ipInfoService = ipInfoService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] bool forceRefresh = false)
    {
        var externalIpv4 = await _ipInfoService.GetExternalIpv4Async(forceRefresh);
        var externalIpv6 = _ipInfoService.GetBestExternalIpv6();
        var adapters = _ipInfoService.GetAllNetworkAdapters();

        var response = new IpSummaryResponse
        {
            ExternalIpv4 = externalIpv4,
            ExternalIpv6 = externalIpv6,
            AdapterCount = adapters.Count
        };

        return Ok(new ApiResponse<IpSummaryResponse>()
        {
            Code = 200,
            Message = "Success",
            Data = response
        });
    }

    [HttpGet("details")]
    public IActionResult GetDetails()
    {
        var adapters = _ipInfoService.GetAllNetworkAdapters();

        return Ok(new ApiResponse<object>()
        {
            Code = 200,
            Message = "Success",
            Data = adapters
        });
    }
}
