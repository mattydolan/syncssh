using System.Net.Http;
using System.Threading.Tasks;


namespace emriweb.Handlers
{
    /// <summary>
    /// Purpose of this handler is to intercept messages.
    /// If the method is post extract the message and make it available
    /// for action filters.
    /// </summary>
    public class HttpPostDataHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                        System.Threading.CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post)
            {
                request.Properties.Add("rawpostdata", request.Content.ReadAsStringAsync().Result);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}