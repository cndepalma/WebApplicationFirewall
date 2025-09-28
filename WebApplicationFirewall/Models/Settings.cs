public class Settings
{
    public bool RateLimitEnabled { get; set; } = true;
    public bool BlockedIpsEnabled { get; set; } = true;
    public bool SqlInjectionDetectionEnabled { get; set; } = true;
}