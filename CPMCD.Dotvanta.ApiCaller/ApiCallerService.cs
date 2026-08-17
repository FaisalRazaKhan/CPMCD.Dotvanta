using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CPMCD.Dotvanta.ApiCaller.Interfaces;
using CPMCD.Dotvanta.ApiCaller.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace CPMCD.Dotvanta.ApiCaller
{
    /// <summary>
    /// Universal HTTP caller - MAUI, Xamarin, Windows (WPF/WinForms) aur ASP.NET Core Web app,
    /// sab isi service ko use kar sakte hain. Platform-specific koi dependency nahi hai.
    /// </summary>
    public class ApiCallerService : IApiCaller
    {
        private readonly ApiCallerConfig _config;
        private readonly JsonSerializerSettings _serializerSettings;

        public ApiCallerService(ApiCallerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        public Task<ApiResponse<TResult>> GetAsync<TResult>(string endpoint, string token = null)
            => CallAsync<object, TResult>(HttpMethodType.Get, endpoint, null, token);

        public Task<ApiResponse<TResult>> PostAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null)
            => CallAsync<TRequest, TResult>(HttpMethodType.Post, endpoint, data, token);

        public Task<ApiResponse<TResult>> PutAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null)
            => CallAsync<TRequest, TResult>(HttpMethodType.Put, endpoint, data, token);

        public Task<ApiResponse<TResult>> DeleteAsync<TResult>(string endpoint, string token = null)
            => CallAsync<object, TResult>(HttpMethodType.Delete, endpoint, null, token);

        public async Task<ApiResponse<TResult>> CallAsync<TRequest, TResult>(
            HttpMethodType method, string endpoint, TRequest data = default, string token = null)
        {
            var result = new ApiResponse<TResult>();
            var uri = BuildUri(endpoint);

            using (var httpClient = CreateHttpClient(token))
            {
                try
                {
                    HttpResponseMessage response;

                    switch (method)
                    {
                        case HttpMethodType.Get:
                            response = await httpClient.GetAsync(uri).ConfigureAwait(false);
                            break;

                        case HttpMethodType.Post:
                            response = await httpClient.PostAsync(uri, BuildContent(data)).ConfigureAwait(false);
                            break;

                        case HttpMethodType.Put:
                            response = await httpClient.PutAsync(uri, BuildContent(data)).ConfigureAwait(false);
                            break;

                        case HttpMethodType.Delete:
                            response = await httpClient.DeleteAsync(uri).ConfigureAwait(false);
                            break;

                        default:
                            throw new NotSupportedException($"HTTP method '{method}' supported nahi hai.");
                    }

                    var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    result.StatusCode = (int)response.StatusCode;
                    result.IsSuccess = response.IsSuccessStatusCode;

                    if (!string.IsNullOrWhiteSpace(raw))
                    {
                        try
                        {
                            result.Data = JsonConvert.DeserializeObject<TResult>(raw, _serializerSettings);
                        }
                        catch (JsonException)
                        {
                            // Response JSON shape expected TResult se match nahi hui.
                            result.ErrorDetails = raw;
                        }
                    }

                    if (!result.IsSuccess)
                    {
                        result.Message = response.ReasonPhrase;
                        result.ErrorDetails ??= raw;
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.StatusCode = 0;
                    result.Message = "Request fail ho gayi.";
                    result.ErrorDetails = ex.Message + ex.StackTrace;
                }
            }

            return result;
        }

        private StringContent BuildContent<TRequest>(TRequest data)
        {
            var json = JsonConvert.SerializeObject(data, _serializerSettings);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        private Uri BuildUri(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(_config.BaseUrl))
                throw new InvalidOperationException("ApiCallerConfig.BaseUrl set nahi hai.");

            var baseUrl = _config.BaseUrl.EndsWith("/") ? _config.BaseUrl : _config.BaseUrl + "/";
            var relative = endpoint.TrimStart('/');
            return new Uri(baseUrl + relative);
        }

        private HttpClient CreateHttpClient(string token)
        {
            var handler = new HttpClientHandler();

            if (_config.BypassSslValidation)
            {
                handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;
            }

            var httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(_config.TimeoutMinutes)
            };

            var jwt = !string.IsNullOrWhiteSpace(token) ? token : _config.Token;
            if (!string.IsNullOrWhiteSpace(jwt))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }

            httpClient.DefaultRequestHeaders.Add("applicationType", _config.ApplicationType);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var header in _config.DefaultHeaders)
            {
                if (!httpClient.DefaultRequestHeaders.Contains(header.Key))
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return httpClient;
        }
    }
}
