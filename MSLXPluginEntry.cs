using Microsoft.AspNetCore.Mvc.ApplicationParts;
using MSLX.SDK;

[assembly: ApplicationPart("MSLX.Plugin.IPInfo")]

namespace MSLX.Plugin.IPInfo;

public class MSLXPluginEntry : IPlugin
{
    public static MSLXPluginEntry Instance { get; private set; }
    public string Id => "mslx-plugin-ipinfo"; 
    public string Name => "IP 信息显示";
    public string Description => "实时显示宿主机公网 IPv4/IPv6 与网卡详细网络信息。";
    public string Version => "1.0.0";
    public string Icon => "icon_77.png";
    public string MinSDKVersion => "1.5.2";
    public string Developer => "xiaoyu";
    public string AuthorUrl => "https://github.com/luluxiaoyu/mslx-plugin-ipinfo";
    public string PluginUrl => "https://mslx-plugins.mslmc.net/plugins/mslx-plugin-ipinfo";

    public async void OnPluginInitialize(IServiceProvider serviceProvider)
    {
        Instance = this;
    }

    public async void OnLoad()
    {
        SDK.MSLX.Logger.Info("mslx-plugin-ipinfo 载入成功~");
    }

    public void OnUnload() {
        SDK.MSLX.Logger.Info("mslx-plugin-ipinfo 卸载成功~");
    }

    public void OnRegisterServices(IServiceCollection services)
    {
        // 注册服务
        services.AddSingleton<Services.IpInfoService>();
    }
}