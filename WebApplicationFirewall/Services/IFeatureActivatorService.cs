public interface IFeatureActivatorService
{
    Settings Settings { get; }
    void UpdateSettings(Settings newSettings);
}