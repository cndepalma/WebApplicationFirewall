namespace WebApplicationFirewall.Helpers
{
    public interface IStorageHelper
    {
        void SaveIps(List<string> ips);

        List<string> LoadIps();
    }
}