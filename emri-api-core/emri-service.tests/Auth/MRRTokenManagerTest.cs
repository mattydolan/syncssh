using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using emri_service.Auth;
using Microsoft.Extensions.Configuration;
using Moq;
using DddsUtils.Logging.NetStandard;

namespace emri_service.tests
{
	public class MRRTokenManagerTest
	{
        private readonly IConfigurationRoot config;
        private readonly IMRRTokenManager _tokenManager;

        public MRRTokenManagerTest()
		{
            config = ConfigurationHelpers.GetConfigurationRoot();
            var logFactory = new Mock<LogFactory>().Object;
            _tokenManager = new MRRTokenManager(config, logFactory);
        }

		[Fact]
        public async Task TokenManager_Generation()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"{config["MRR:MrrServiceUrl"]}/MA/projects");
            request.Headers.Add("User-Agent", "EMRI-API");
            request.Headers.Add("token", _tokenManager.GetMRRToken());

            var client = new HttpClient();

            //Act
            var response = await client.SendAsync(request);

            //Assert
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
