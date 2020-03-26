using System.IO;
using Asmx.Models;

namespace Asmx.Services
{
    public class ConfigurationService
    {
        public static ApiConfiguration ReadConfig()
        {
            return new ApiConfiguration
            {
                Authority = "https://login.microsoftonline.com/e205bfab-7c3a-4369-86f3-030001469257",
                ClientId = "8d5b3618-7f3f-4e09-93da-072703a47193",
            };
        }
    }
}
