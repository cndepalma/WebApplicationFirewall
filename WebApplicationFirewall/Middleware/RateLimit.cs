using System.Collections.Concurrent;

public class RateLimit
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimit> _logger;
    private static readonly ConcurrentDictionary<string, (int Count, DateTime Timestamp)> RateLimits = new();

    private const int RequestLimit = 5;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromSeconds(15);
    private readonly IFeatureActivatorService _settingsService;

    public RateLimit(RequestDelegate next, ILogger<RateLimit> logger, IFeatureActivatorService settingsService)
    {
        _next = next;
        _logger = logger;
        _settingsService = settingsService;
    }

    public async Task Invoke(HttpContext context)
    {
        if (_settingsService.Settings.RateLimitEnabled)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            if (clientIp != null)
            {
                var now = DateTime.UtcNow;
                if (RateLimits.TryGetValue(clientIp, out var entry))
                {
                    if (now - entry.Timestamp < TimeWindow)
                    {
                        if (entry.Count >= RequestLimit)
                        {
                            _logger.LogWarning($"Rate limit exceeded for IP: {clientIp}");
                            context.Response.StatusCode = 429;
                            await context.Response.WriteAsync("Please try again later.");
                            return;
                        }

                        RateLimits[clientIp] = (entry.Count + 1, entry.Timestamp);
                    }
                    else
                    {
                        RateLimits[clientIp] = (1, now);
                    }
                }
                else
                {
                    RateLimits[clientIp] = (1, now);
                }
            }
        }
        await _next(context);
    }
}