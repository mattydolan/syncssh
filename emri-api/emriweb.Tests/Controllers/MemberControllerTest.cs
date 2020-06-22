using System;
using System.Collections.Generic;
using Moq;
using NLog;
using Xunit;

using emriweb.Controllers;

using DddsUtils.Logging;


namespace emriweb.Tests.Controllers
{

    public class MemberControllerTest
    {
        private Mock<ILogger> logger;
        private Mock<ILogFactory> logFactory;

        public MemberControllerTest()
        {
            logFactory = new Mock<ILogFactory>();
            logger = new Mock<ILogger>();
            logFactory.Setup(s => s.GetLogger(It.IsAny<string>())).Returns(logger.Object);
        }

        [Fact]
        public void MemberControllerConstructorTest()
        {
            var member = new MemberController(logFactory.Object);
            Assert.IsType<MemberController>(member);
        }

        [Fact]
        public void MemberControllerGetTest()
        {
            var member = new MemberController(logFactory.Object);
            var result = member.Get();

            Assert.IsAssignableFrom<IEnumerable<string>>(result);
        }
    }
}
