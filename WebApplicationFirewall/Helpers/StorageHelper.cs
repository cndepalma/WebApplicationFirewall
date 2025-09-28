using Newtonsoft.Json;
using WebApplicationFirewall.Models;

namespace WebApplicationFirewall.Helpers
{
    public class StorageHelper : IStorageHelper
    {
        public StorageHelper() { }
        private string filePath = Path.Combine("Data", "IpList.json");

        public void SaveIps(List<string> ips)
        {
            var ipList = new IpList { Ips = ips };
            string json = JsonConvert.SerializeObject(ipList, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public List<string> LoadIps()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var ipList = JsonConvert.DeserializeObject<IpList>(json);
                return ipList?.Ips ?? new List<string>();
            }
            return new List<string>();
        }
    }
}