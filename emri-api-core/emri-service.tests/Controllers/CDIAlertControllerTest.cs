using System.Collections.Generic;
using Moq;
using NLog;
using Xunit;
using emri_service.Controllers;
using DddsUtils.Logging.NetStandard;
using MassTransit;
using emri_service.Managers;
using System.Threading.Tasks;
using emri_models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace emri_service.tests.Controllers
{
    public class CDIAlertControllerTest
    {
        private Mock<ILogger> logger;
        private Mock<ILogFactory> logFactory;
        private Mock<IBusControl> busControl;

        public CDIAlertControllerTest()
        {
            logFactory = new Mock<ILogFactory>();
            logger = new Mock<ILogger>();
            busControl = new Mock<IBusControl>();
            logFactory.Setup(s => s.GetLogger(It.IsAny<string>())).Returns(logger.Object);
        }

        [Fact]
        public void CDIAlertControllerConstructorTest()
        {
            Mock<ICDIAlertManager> cdiMgr = new Mock<ICDIAlertManager>();
            var alertController = new CDIAlertController(busControl.Object, logFactory.Object, cdiMgr.Object);
            Assert.IsAssignableFrom<CDIAlertController>(alertController);
        }

        [Fact]
        public void CDIAlertControllerPostCDIAlertValidationFailTest()
        {
            var errorDictiionary = new Dictionary<string, string>();
            errorDictiionary.Add("a", "This is a test message");
            Mock<ICDIAlertManager> cdiMgr = new Mock<ICDIAlertManager>();
            cdiMgr.Setup(x => x.ValidateCDIAlertEntityID(It.IsAny<int>()))
                .ReturnsAsync(errorDictiionary);

            CDIAlertResponseDTO dto = new CDIAlertResponseDTO { CDIAlertEntityID = 1 };

            var alertController = new CDIAlertController(busControl.Object, logFactory.Object, cdiMgr.Object);
            var result = alertController.Post(1, dto);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.True("This is a test message".Equals(((BadRequestObjectResult)result).Value.ToString(), System.StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void CDIAlertControllerPostCDIAlertTrackingRecordValidationFailTest()
        {
            ActionContext actionContext = new ActionContext();
            var errorDictiionary = new Dictionary<string, string>();
            Mock<ICDIAlertManager> cdiMgr = new Mock<ICDIAlertManager>();
            cdiMgr.Setup(x => x.ValidateCDIAlertEntityID(It.IsAny<int>()))
                .ReturnsAsync(errorDictiionary);
            cdiMgr.Setup(x => x.InsertEMRTrackingRecord(It.IsAny<CDIAlertResponseDTO>()))
                .Returns(0);
            CDIAlertResponseDTO dto = new CDIAlertResponseDTO { CDIAlertEntityID = 1 };

            var alertController = new CDIAlertController(busControl.Object, logFactory.Object, cdiMgr.Object);
            var result = alertController.Post(1, dto);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.True("Something went wrong".Equals(((BadRequestObjectResult)result).Value.ToString(), System.StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void CDIAlertControllerPostCreatedTest()
        {
            var errorDictiionary = new Dictionary<string, string>();
            Mock<ICDIAlertManager> cdiMgr = new Mock<ICDIAlertManager>();
            cdiMgr.Setup(x => x.ValidateCDIAlertEntityID(It.IsAny<int>()))
                .ReturnsAsync(errorDictiionary);
            cdiMgr.Setup(x => x.InsertEMRTrackingRecord(It.IsAny<CDIAlertResponseDTO>()))
                .Returns(1);
            CDIAlertResponseDTO dto = new CDIAlertResponseDTO { CDIAlertEntityID = 1 };

            var alertController = new CDIAlertController(busControl.Object, logFactory.Object, cdiMgr.Object);
            var result = alertController.Post(1, dto);
            Assert.IsType<CreatedResult>(result);
            dynamic d = ((CreatedResult)result).Value;
            var prop = ((object)d).GetType().GetProperty("id");
            int id = (int)prop.GetValue(d);
            Assert.True(id == 1);
        }

        [Fact]
        public void CDIAlertControllerGetOkResultTest()
        {
            Mock<ICDIAlertManager> cdiMgr = new Mock<ICDIAlertManager>();
            var alertController = new CDIAlertController(busControl.Object, logFactory.Object, cdiMgr.Object);
            var result = alertController.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.IsType<OkResult>(result);
        }

    }
}
