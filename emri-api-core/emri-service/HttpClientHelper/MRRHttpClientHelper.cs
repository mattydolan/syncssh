using System;

using System.Net.Http;
using System.Net.Http.Headers;


namespace emri_service.HttpClientHelper
{
    public static class MRRHttpClientHelper
    {
        public static void AddMRRDefaultHeaders(this HttpClient httpClient, string token = null)
        {

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.DefaultRequestHeaders.Remove("token");
            httpClient.DefaultRequestHeaders.Add("token", token);
        }
    }
}
