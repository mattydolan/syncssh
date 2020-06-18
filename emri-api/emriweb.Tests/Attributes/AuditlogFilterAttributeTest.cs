using System;
using Moq;
using Xunit;
using NLog;
using DddsUtils.Logging;
using emriweb.Attributes;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;


namespace emriweb.Tests.Attributes
{

    public class AuditlogFilterAttributeTest
    {
        private Mock<ILogger> logger;
        private Mock<ILogFactory> logFactory;

        public AuditlogFilterAttributeTest()
        {
            logFactory = new Mock<ILogFactory>();
            logger = new Mock<ILogger>();
            logFactory.Setup(s => s.GetLogger(It.IsAny<string>())).Returns(logger.Object);
        }

        [Fact]
        public void AuditLogFilterAttributeGetTest()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK };
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost")
            };

            HttpControllerContext controllerContext = new HttpControllerContext { Request = requestMessage };

            HttpActionContext actionContext = new HttpActionContext { ControllerContext = controllerContext, Response = responseMessage };

            HttpActionExecutedContext aec = new HttpActionExecutedContext { ActionContext = actionContext };

            var auditFilter = new AuditLogFilterAttribute();
            auditFilter.AuditLogFactory = logFactory.Object;
            auditFilter.OnActionExecuted(aec);
        }

        [Fact]
        public void AuditLogFilterAttributePostTest()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("This is for attribute response test")
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost"),
                Content = new StringContent("This is for attribute test")
            };
            requestMessage.Properties.Add("rawpostdata", "This is for attribute test");

            HttpControllerContext controllerContext = new HttpControllerContext { Request = requestMessage };

            HttpActionContext actionContext = new HttpActionContext { ControllerContext = controllerContext, Response = responseMessage };

            HttpActionExecutedContext aec = new HttpActionExecutedContext { ActionContext = actionContext };

            var auditFilter = new AuditLogFilterAttribute();
            auditFilter.AuditLogFactory = logFactory.Object;
            auditFilter.OnActionExecuted(aec);
        }
    }
}
