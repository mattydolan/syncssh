using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Moq.Protected;
using NLog;
using Xunit;
using DddsUtils.Logging.NetStandard;
using System.Net.Http;
using emri_service.Auth;
using Microsoft.Extensions.Configuration;
using emri_repository;
using emri_models.Dto;
using emri_service.Managers;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace emri_service.tests.Managers
{
    public class CDIAlertManagerTest
    {
        private Mock<ILogger> logger;
        private Mock<ILogFactory> logFactory;

        public CDIAlertManagerTest()
        {
            logFactory = new Mock<ILogFactory>();
            logger = new Mock<ILogger>();
            logFactory.Setup(s => s.GetLogger(It.IsAny<string>())).Returns(logger.Object);
        }

        [Fact]
        public void CDIAlertManagerConstructorTest()
        {
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(clientHandlerMock.Object);

            httpFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var tm = new Mock<IMRRTokenManager>();
            tm.Setup(y => y.GetMRRToken()).Returns(It.IsAny<string>());

            var cfg = new Mock<IConfiguration>();
            var emrRepo = new Mock<IEMRTrackingRepository>();
            //emrRepo.Setup(z => z.InsertEMRTrackingRecord(It.IsAny<EMRTrackingDTO>()))
            //    .Returns(1);

            var mgrObj = new CDIAlertMananger(logFactory.Object,
                httpFactory.Object,
                tm.Object, cfg.Object, emrRepo.Object);
            Assert.IsAssignableFrom<CDIAlertMananger>(mgrObj);
        }

        [Fact]
        public async void CDIAlertManagerValidateMemberTest()
        {
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(clientHandlerMock.Object);

            httpFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var tm = new Mock<IMRRTokenManager>();
            tm.Setup(y => y.GetMRRToken()).Returns(It.IsAny<string>());

            var cfg = new Mock<IConfiguration>();
            cfg.Setup(x => x["MRR:mrrMemberUrl"])
               .Returns("http://localhost/");

            var emrRepo = new Mock<IEMRTrackingRepository>();
            //emrRepo.Setup(z => z.InsertEMRTrackingRecord(It.IsAny<EMRTrackingDTO>()))
            //    .Returns(1);

            var mgrObj = new CDIAlertMananger(logFactory.Object,
                httpFactory.Object,
                tm.Object, cfg.Object, emrRepo.Object);

            var response = await mgrObj.ValidateMember("ABC");

            Assert.IsAssignableFrom<CDIAlertMananger>(mgrObj);
            Assert.IsAssignableFrom<Dictionary<string, string>>(response);
        }

        [Fact]
        public async void CDIAlertManagerValidateMemberNotFoundTest()
        {
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound))
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(clientHandlerMock.Object);

            httpFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var tm = new Mock<IMRRTokenManager>();
            tm.Setup(y => y.GetMRRToken()).Returns(It.IsAny<string>());

            var cfg = new Mock<IConfiguration>();
            cfg.Setup(x => x["MRR:mrrMemberUrl"])
               .Returns("http://localhost/");

            var emrRepo = new Mock<IEMRTrackingRepository>();
            //emrRepo.Setup(z => z.InsertEMRTrackingRecord(It.IsAny<EMRTrackingDTO>()))
            //    .Returns(1);

            var mgrObj = new CDIAlertMananger(logFactory.Object,
                httpFactory.Object,
                tm.Object, cfg.Object, emrRepo.Object);

            var response = await mgrObj.ValidateMember("ABC");

            Assert.IsAssignableFrom<CDIAlertMananger>(mgrObj);
            Assert.IsAssignableFrom<Dictionary<string, string>>(response);
            Assert.True(response.Count > 0);
        }

        [Fact]
        public async void CDIAlertManagerValidateCDIAlertEntityTest()
        {
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(clientHandlerMock.Object);

            httpFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var tm = new Mock<IMRRTokenManager>();
            tm.Setup(y => y.GetMRRToken()).Returns(It.IsAny<string>());

            var cfg = new Mock<IConfiguration>();
            cfg.Setup(x => x["MRR:mrrServiceUrl"])
               .Returns("http://localhost/");

            var emrRepo = new Mock<IEMRTrackingRepository>();
            emrRepo.Setup(z => z.InsertEMRTrackingRecord(It.IsAny<EMRTrackingDTO>()))
                .Returns(1);

            var mgrObj = new CDIAlertMananger(logFactory.Object,
                httpFactory.Object,
                tm.Object, cfg.Object, emrRepo.Object);

            var response = await mgrObj.ValidateCDIAlertEntityID(1);

            Assert.IsAssignableFrom<CDIAlertMananger>(mgrObj);
            Assert.IsAssignableFrom<Dictionary<string, string>>(response);
        }

        [Fact]
        public async void CDIAlertManagerValidateCDIAlertEntityidNotFoundTest()
        {
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound))
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(clientHandlerMock.Object);

            httpFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var tm = new Mock<IMRRTokenManager>();
            tm.Setup(y => y.GetMRRToken()).Returns(It.IsAny<string>());

            var cfg = new Mock<IConfiguration>();
            cfg.Setup(x => x["MRR:mrrServiceUrl"])
               .Returns("http://localhost/");

            var emrRepo = new Mock<IEMRTrackingRepository>();
            //emrRepo.Setup(z => z.InsertEMRTrackingRecord(It.IsAny<EMRTrackingDTO>()))
            //    .Returns(1);

            var mgrObj = new CDIAlertMananger(logFactory.Object,
                httpFactory.Object,
                tm.Object, cfg.Object, emrRepo.Object);

            var response = await mgrObj.ValidateCDIAlertEntityID(1);

            Assert.IsAssignableFrom<CDIAlertMananger>(mgrObj);
            Assert.IsAssignableFrom<Dictionary<string, string>>(response);
            Assert.True(response.Count > 0);
        }

        [Fact]
        public void CDIAlertManagerInsertTrackingIdZeroTest()
        {
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound))
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(clientHandlerMock.Object);

            httpFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var tm = new Mock<IMRRTokenManager>();
            tm.Setup(y => y.GetMRRToken()).Returns(It.IsAny<string>());

            var cfg = new Mock<IConfiguration>();
            cfg.Setup(x => x["MRR:mrrServiceUrl"])
               .Returns("http://localhost/");

            var emrRepo = new Mock<IEMRTrackingRepository>();
            emrRepo.Setup(z => z.InsertEMRTrackingRecord(It.IsAny<EMRTrackingDTO>()))
                .Returns(0);

            var mgrObj = new CDIAlertMananger(logFactory.Object,
                httpFactory.Object,
                tm.Object, cfg.Object, emrRepo.Object);

            var result = mgrObj.InsertEMRTrackingRecord(new CDIAlertResponseDTO());

            Assert.IsAssignableFrom<CDIAlertMananger>(mgrObj);
            Assert.True(result == 0);
        }

        [Fact]
        public void CDIAlertManagerInsertTrackingIdGreaterThanZeroTest()
        {
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientHandlerMock = new Mock<DelegatingHandler>();
            clientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound))
                .Verifiable();
            clientHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(clientHandlerMock.Object);

            httpFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var tm = new Mock<IMRRTokenManager>();
            tm.Setup(y => y.GetMRRToken()).Returns(It.IsAny<string>());

            var cfg = new Mock<IConfiguration>();
            cfg.Setup(x => x["MRR:mrrServiceUrl"])
               .Returns("http://localhost/");

            var emrRepo = new Mock<IEMRTrackingRepository>();
            emrRepo.Setup(z => z.InsertEMRTrackingRecord(It.IsAny<EMRTrackingDTO>()))
                .Returns(1);

            var mgrObj = new CDIAlertMananger(logFactory.Object,
                httpFactory.Object,
                tm.Object, cfg.Object, emrRepo.Object);

            var result = mgrObj.InsertEMRTrackingRecord(new CDIAlertResponseDTO());

            Assert.IsAssignableFrom<CDIAlertMananger>(mgrObj);
            Assert.True(result == 1);
        }
    }
}
