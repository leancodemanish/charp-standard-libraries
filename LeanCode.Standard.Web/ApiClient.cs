using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LeanCode.Standard.Logging;
using LeanCode.Standard.Serialization;

namespace LeanCode.Standard.Web
{
    public class ApiClient
    {
        private readonly Lazy<WebClient> webClient = new Lazy<WebClient>(() => new WebClient());
        private static ILogger _logger = LoggerFactoryFacade.GetLogger(typeof(ApiClient));

        public static async Task<string> GetAsync(string url, CancellationToken cToken)
        {
            HttpResponseMessage resp = null;
            string response = null;
            try
            {
                using (var client = CreateClient())
                {
                    _logger.Info($"Calling get for {url} via {client.BaseAddress}");

                    resp = await client.GetAsync(url, cToken);

                    if (resp != null && resp.IsSuccessStatusCode)
                    {
                        var contentResponse = resp.Content;
                        response = await contentResponse.ReadAsStringAsync();
                        return response;
                    }

                    var responseCode = resp == null ? "null" : resp.StatusCode.ToString();
                    _logger.Error($"Failed to run query: {url} ({responseCode}). Response: {resp}");
                    return response;
                }
            }
            catch (TaskCanceledException)
            {
                _logger.Info($"Task cancelled when running query {url}.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Unhandled error when running query {url}: {ex}. Response: {resp}");
            }

            return response;
        }

        public async Task<TResponse> CallPostApi<TResponse>(string url, object request)
        {
            webClient.Value.Headers["Content-Type"] = "application/json";

            var response = await webClient.Value.UploadStringTaskAsync(new Uri(url), "POST", JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            return JsonConvert.DeserializeObject<TResponse>(response);
        }

        public static async Task<TResponse> PostAsync<TRequest, TResponse>(string baseUrl, string controllerUrl, TRequest request, CancellationToken cToken)
        {
            HttpResponseMessage resp = null;

            try
            {
                using (var client = CreateClient())
                {
                    _logger.Info($"Calling post for {controllerUrl} via {client.BaseAddress}");
                    var stringContent = JsonConvert.SerializeObject(request);
                    using (StringContent content = new StringContent(stringContent, Encoding.UTF8, "application/json"))
                    {
                        resp = await client.PostAsync(controllerUrl, content, cToken);
                        if (resp.IsSuccessStatusCode)
                        {
                            var contentResponse = resp.Content;
                            var response = await contentResponse.ReadAsStringAsync();
                            JToken jToken = JToken.Parse(response);//To validate and to get rid of undesired escape sequences
                            return Serialization.JsonSerializer.Deserialize<TResponse>((string)jToken);
                        }

                        _logger.Error($"Failed to run query: {controllerUrl} ({resp.StatusCode}). Response: {resp}");
                        return default(TResponse);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                _logger.Info($"Task cancelled when running query {controllerUrl}.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Unhandled error when running query {controllerUrl}: {ex}. Response: {resp}");
            }

            return default(TResponse);
        }

        public static TResponse CallGetMethod<TResponse>(string url, string baseApi)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseApi);
                var result = client.GetAsync(url).Result;
                result.EnsureSuccessStatusCode();
                var resultstring = result.Content.ReadAsStringAsync().Result;
                if (typeof(TResponse) == typeof(string))
                {
                    return (TResponse)(object)resultstring;
                }
                else
                {
                    TResponse resultContent = Serialization.JsonSerializer.Deserialize<TResponse>(resultstring);
                    return resultContent;
                }

            }
        }

        private static HttpClient CreateClient()
        {
            IWebProxy proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                CookieContainer = new CookieContainer(),
                Proxy = proxy
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(30)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }


    }
}
