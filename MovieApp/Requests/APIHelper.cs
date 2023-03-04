using Polly;
using Polly.Retry;
using System.Net.Http;

namespace MovieApp.Requests
{
    public class APIHelper
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
        //public async Task<BvnDetailsRespObj> validateBvnDetails(string url)
        //{
        //    BvnDetailsRespObj responseObj = new BvnDetailsRespObj();
        //    var flutterWaveClient = _httpClientFactory.CreateClient("FLUTTERWAVE");
        //    var keys = await _serverRequest.GetFlutterWaveKeys();
        //    flutterWaveClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + keys.keys.secret_keys);

        //    return await _retryPolicy.ExecuteAsync(async () =>
        //    {
        //        try
        //        {
        //            result = await flutterWaveClient.GetAsync(url);
        //            if (!result.IsSuccessStatusCode)
        //            {
        //                var data1 = await result.Content.ReadAsStringAsync();
        //                responseObj = JsonConvert.DeserializeObject<BvnDetailsRespObj>(data1);
        //                return new BvnDetailsRespObj
        //                {
        //                    status = "BVN: " + responseObj.message
        //                };
        //            }
        //            var data = await result.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<BvnDetailsRespObj>(data);
        //        }
        //        catch (Exception ex) { throw ex; }
        //        if (responseObj == null)
        //        {
        //            return new BvnDetailsRespObj
        //            {
        //                status = "Not Successful"
        //            };
        //        }
        //        if (responseObj.status == "success")
        //        {
        //            return new BvnDetailsRespObj
        //            {
        //                status = responseObj.status,
        //                data = responseObj.data,
        //                message = responseObj.message
        //            };
        //        }
        //        return new BvnDetailsRespObj
        //        {
        //            status = "BVN: " + responseObj.message
        //        };
        //    });
        //}
    }
}
