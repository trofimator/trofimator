using System.IO;
using Asmx.Models;

namespace Asmx.Services
{
    public class ConfigurationService
    {
        public static Config ReadConfig()
        {
            return new Config
            {
                Authority = "",
                ClientId = "",
                ValidAudience = "",
            };
        }
    }
}
