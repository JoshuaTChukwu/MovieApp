using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Net.Http;
using static MovieApp.Contracts.Common.OMDBAPIRequestObjs;

namespace MovieApp.Requests
{
    public class APIHelper : IAPIHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsyncRetryPolicy _retryPolicy;
        private const int maxRetryTimes = 10;
        public APIHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _retryPolicy = Policy.Handle<HttpRequestException>()

             .WaitAndRetryAsync(maxRetryTimes, times =>

             TimeSpan.FromSeconds(times * 2));
        }
        public async Task<OmbdResult> GetMovie(SearchParams search)
        {
            OmbdResult responseObj = new OmbdResult();
            var client = _httpClientFactory.CreateClient("OMDB");
            var url = $"&s={search.SearchValue}&page={search.Page}";

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                   var result = await client.GetAsync(url);
                   
                    var data = await result.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<OmbdResult>(data);
                    if(responseObj == null)
                    {
                        return new OmbdResult();
                    }
                    return responseObj;
                }
                catch (Exception) { throw; }
               
               
            });
        }
    }

    public interface IAPIHelper
    {
        Task<OmbdResult> GetMovie(SearchParams search);
    }
}
