using System.Collections.Generic;
using Moq;
using NLog;
using Xunit;
using emri_service.Controllers;
using DddsUtils.Logging.NetStandard;

namespace emri_service.tests.Controllers
{
	public class ProviderControllerTest
    {
        private readonly Mock<ILogger> logger;
        private readonly Mock<ILogFactory> logFactory;

        public ProviderControllerTest()
        {
            logFactory = new Mock<ILogFactory>();
            logger = new Mock<ILogger>();
            logFactory.Setup(s => s.GetLogger(It.IsAny<string>())).Returns(logger.Object);
        }

        [Fact]
        public void ProviderControllerConstructorTest()
        {
            var provider = new ProviderController(logFactory.Object);
            Assert.IsType<ProviderController>(provider);
        }

        [Fact]
        public void ProviderControllerGetTest()
        {
            var provider = new ProviderController(logFactory.Object);
            var result = provider.Get();

            Assert.IsAssignableFrom<IEnumerable<string>>(result);
        }
    }
}
