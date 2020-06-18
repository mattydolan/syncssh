using Microsoft.Extensions.Configuration;

namespace emri_service.tests
{
	public class ConfigurationHelpers
	{
        public static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
    }
}
