using Moq;
using NLog;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using emri_service.Middleware;
using DddsUtils.Logging.NetStandard;

namespace emri_service.tests.Middleware
{
	public class AuditLogMiddlewareTest
    {
        private readonly Mock<ILogger> logger;
        private readonly Mock<ILogFactory> logFactory;

        public AuditLogMiddlewareTest()
        {
            logFactory = new Mock<ILogFactory>();
            logger = new Mock<ILogger>();
            logFactory.Setup(s => s.GetLogger(It.IsAny<string>())).Returns(logger.Object);
        }

        [Fact]
        public async Task AuditLogMiddleware_Get()
        {
            //Arrange
            var middleware = new AuditLogMiddleware(
                next: (innerHttpContext) =>
                {
                    return Task.CompletedTask;
                }
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = HttpMethods.Get;
            httpContext.Request.Path = "/MemberController/Get";
            httpContext.Response.StatusCode = StatusCodes.Status200OK;

            //Act
            await middleware.Invoke(httpContext, logFactory.Object);

            //Assert (via inspection)
            // If a non-mock LogFactory is used with this method, check the AuditLog table --
            // this should not have created a record (only POSTs are logged).
        }

        [Fact]
        public async Task AuditLogMiddleware_Post()
        {
            //Arrange
            var middleware = new AuditLogMiddleware(
                next: (innerHttpContext) =>
                {
                    return Task.CompletedTask;
                }
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = HttpMethods.Post;
            httpContext.Request.Path = "/MemberController/Post";
            httpContext.Request.Body = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(httpContext.Request.Body, null, -1, true)) //leaveOpen is key
            {
                sw.WriteLine("a good test request body");
            }

            httpContext.Response.Body = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(httpContext.Response.Body, null, -1, true))
            {
                sw.WriteLine("a better test response body");
            }
            httpContext.Response.StatusCode = StatusCodes.Status200OK;

            //Act
            await middleware.Invoke(httpContext, logFactory.Object);

            //Assert (via inspection)
            // If a non-mock LogFactory is used with this method, check the AuditLog table --
            // this should have created a record.
        }
    }
}
