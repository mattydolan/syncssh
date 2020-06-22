using System;
using System.Collections.Generic;
using Moq;
using NLog;
using Xunit;

using emriweb.Controllers;

using DddsUtils.Logging;

namespace emriweb.Tests.Controllers
{

    public class ProviderControllerTest
    {

        private Mock<ILogger> logger;
        private Mock<ILogFactory> logFactory;

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
