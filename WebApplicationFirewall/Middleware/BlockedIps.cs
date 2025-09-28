using WebApplicationFirewall.Helpers;

public class BlockedIps
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BlockedIps> _logger;
    private readonly IStorageHelper _storageHelper;
    private readonly IFeatureActivatorService _settingsService;

    public BlockedIps(RequestDelegate next, ILogger<BlockedIps> logger, IStorageHelper storageHelper, IFeatureActivatorService settingsService)
    {
        _next = next;
        _logger = logger;
        _storageHelper = storageHelper ?? throw new ArgumentNullException(nameof(storageHelper));
        _settingsService = settingsService;
    }

    public async Task Invoke(HttpContext context)
    {
        if (_settingsService.Settings.BlockedIpsEnabled)
        {
            string clientIp = context.Connection.RemoteIpAddress?.ToString();
            var loadedIps = _storageHelper.LoadIps();

            if (clientIp != null && loadedIps.Contains(clientIp))
            {
                _logger.LogWarning($"Blocked request from IP: {clientIp}");
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("403 Forbidden: Your IP has been blocked.");
                return;
            }
        }
        await _next(context);
    }
}