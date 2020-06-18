using System;
using Xunit;
using Moq;
using Moq.Protected;

using emriweb.Handlers;
using System.Net.Http;
using System.Threading;
using System.Net;
using System.Threading.Tasks;


namespace emriweb.Tests.Handlers
{

    public class HttpPostDataHandlerTest
    {

        [Fact]
        public void HandleRawPostDataGetTest()
        {
            HttpRequestMessage _httpRequest;
            _httpRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            Mock<DelegatingHandler> _testHandler = new Mock<DelegatingHandler>();
            var mockedResult = new HttpResponseMessage(HttpStatusCode.OK);

            _testHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequest, It.IsAny<CancellationToken>())
                .ReturnsAsync(mockedResult);
            var handler = new HttpPostDataHandler
            {
                InnerHandler = _testHandler.Object
            };
            var invoker = new HttpMessageInvoker(handler);
            var result = invoker.SendAsync(_httpRequest, new CancellationToken()).Result;

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void HandleRawPostDataPostTest()
        {
            HttpRequestMessage _httpRequest;
            _httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
            var mockedResult = new HttpResponseMessage(HttpStatusCode.OK);
            Mock<StringContent> content = new Mock<StringContent>(MockBehavior.Loose, "This is a result");
            mockedResult.Content = content.Object;

            _httpRequest.Content = content.Object;
            Mock<DelegatingHandler> _testHandler = new Mock<DelegatingHandler>();

            _testHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequest, It.IsAny<CancellationToken>())
                .ReturnsAsync(mockedResult);

            var handler = new HttpPostDataHandler
            {
                InnerHandler = _testHandler.Object
            };
            var invoker = new HttpMessageInvoker(handler);
            var result = invoker.SendAsync(_httpRequest, new CancellationToken()).Result;

            Assert.IsType<HttpResponseMessage>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            _httpRequest.Properties.TryGetValue("rawpostdata", out object obj);

            Assert.Equal(mockedResult.Content.ReadAsStringAsync().Result, obj);
        }
    }
}
