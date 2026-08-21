using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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
    /// sab isi service ko use kar sakte hain. Platform-specific koi hard dependency nahi hai -
    /// runtime khud sahi native handler (Android/iOS/Windows/Linux) uthata hai kyunki hum
    /// System.Net.Http ka standard HttpClient use karte hain.
    ///
    /// PERFORMANCE: HttpClient/handler process ki lifetime ke liye ek baar banta hai aur BaseUrl +
    /// settings ke hisaab se cache/reuse hota hai — har request pe naya HttpClient/socket banana
    /// (purane version ka sabse bada slowdown + socket-exhaustion bug) yahan fix kiya gaya hai.
    /// </summary>
    public class ApiCallerService : IApiCaller, IDisposable
    {
        // Process-wide pool: ek hi BaseUrl+settings combination ke liye ek hi HttpClient reuse hota
        // hai, chahe app multiple ApiCallerService instances bana le. Isse connections/DNS/TLS
        // handshake baar baar nahi hote -> sab platforms pe consistently fast.
        private static readonly ConcurrentDictionary<string, HttpClient> ClientCache = new ConcurrentDictionary<string, HttpClient>();

        private readonly ApiCallerConfig _config;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly HttpClient _httpClient;

        public ApiCallerService(ApiCallerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(_config.BaseUrl))
                throw new InvalidOperationException("ApiCallerConfig.BaseUrl set nahi hai.");

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());

            _httpClient = GetOrCreateClient(_config);
        }

        private static string BuildCacheKey(ApiCallerConfig config)
        {
            return string.Join("|",
                config.BaseUrl,
                config.ApplicationType,
                config.BypassSslValidation,
                config.TimeoutMinutes);
        }

        private static HttpClient GetOrCreateClient(ApiCallerConfig config)
        {
            var key = BuildCacheKey(config);

            return ClientCache.GetOrAdd(key, _ =>
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                };

                if (config.BypassSslValidation)
                {
                    handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;
                }

                var client = new HttpClient(handler, disposeHandler: true)
                {
                    Timeout = TimeSpan.FromMinutes(config.TimeoutMinutes)
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("applicationType", config.ApplicationType);

                foreach (var header in config.DefaultHeaders)
                {
                    if (!client.DefaultRequestHeaders.Contains(header.Key))
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                return client;
            });
        }

        public Task<ApiResponse<TResult>> GetAsync<TResult>(string endpoint, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
            => CallAsync<object, TResult>(HttpMethodType.Get, endpoint, null, token, headers, cancellationToken);

        public Task<ApiResponse<TResult>> PostAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
            => CallAsync<TRequest, TResult>(HttpMethodType.Post, endpoint, data, token, headers, cancellationToken);

        public Task<ApiResponse<TResult>> PutAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
            => CallAsync<TRequest, TResult>(HttpMethodType.Put, endpoint, data, token, headers, cancellationToken);

        public Task<ApiResponse<TResult>> PatchAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
            => CallAsync<TRequest, TResult>(HttpMethodType.Patch, endpoint, data, token, headers, cancellationToken);

        public Task<ApiResponse<TResult>> DeleteAsync<TResult>(string endpoint, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
            => CallAsync<object, TResult>(HttpMethodType.Delete, endpoint, null, token, headers, cancellationToken);

        public async Task<ApiResponse<TResult>> CallAsync<TRequest, TResult>(
            HttpMethodType method, string endpoint, TRequest data = default, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            var result = new ApiResponse<TResult>();
            var attempt = 0;
            var maxAttempts = _config.EnableAutoRetry ? Math.Max(1, _config.MaxRetryCount + 1) : 1;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                attempt++;
                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(_config.TimeoutMinutes)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
                {
                    try
                    {
                        using (var request = BuildRequest(method, endpoint, data, token, headers))
                        using (var response = await _httpClient
                            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linkedCts.Token)
                            .ConfigureAwait(false))
                        {
                            result.StatusCode = (int)response.StatusCode;
                            result.IsSuccess = response.IsSuccessStatusCode;

                            await ReadResponseBodyAsync(response, result, linkedCts.Token).ConfigureAwait(false);

                            if (!result.IsSuccess)
                            {
                                result.Message = response.ReasonPhrase;

                                if (ShouldRetry(result.StatusCode) && attempt < maxAttempts)
                                {
                                    await DelayBeforeRetry(attempt).ConfigureAwait(false);
                                    continue;
                                }
                            }

                            stopwatch.Stop();
                            result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                            return result;
                        }
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        // Caller ne khud cancel kiya - timeout se alag, retry nahi karna.
                        result.IsSuccess = false;
                        result.StatusCode = 0;
                        result.IsCancelled = true;
                        result.Message = "Request cancelled by caller.";
                        stopwatch.Stop();
                        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        // Config ka TimeoutMinutes exceed hua.
                        if (attempt < maxAttempts && _config.EnableAutoRetry)
                        {
                            await DelayBeforeRetry(attempt).ConfigureAwait(false);
                            continue;
                        }

                        result.IsSuccess = false;
                        result.StatusCode = 0;
                        result.IsTimeout = true;
                        result.Message = $"Request timed out after {_config.TimeoutMinutes} minute(s).";
                        result.ErrorDetails = result.Message;
                        stopwatch.Stop();
                        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                        return result;
                    }
                    catch (Exception ex)
                    {
                        if (attempt < maxAttempts && _config.EnableAutoRetry)
                        {
                            await DelayBeforeRetry(attempt).ConfigureAwait(false);
                            continue;
                        }

                        result.IsSuccess = false;
                        result.StatusCode = 0;
                        result.Message = $"Request failed: {ex.Message}";
                        result.ErrorDetails = ex.ToString();
                        stopwatch.Stop();
                        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                        return result;
                    }
                }
            }
        }

        private static bool ShouldRetry(int statusCode)
        {
            // Transient server-side codes hi retry ke liye eligible - 4xx client errors kabhi retry nahi.
            return statusCode == 408 || statusCode == 429 || statusCode >= 500;
        }

        private async Task DelayBeforeRetry(int attempt)
        {
            var delay = _config.RetryBaseDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
            await Task.Delay(delay).ConfigureAwait(false);
        }

        private HttpRequestMessage BuildRequest<TRequest>(HttpMethodType method, string endpoint, TRequest data,
            string token, Dictionary<string, string> headers)
        {
            var httpMethod = ToHttpMethod(method);
            var request = new HttpRequestMessage(httpMethod, BuildUri(endpoint));

            var jwt = !string.IsNullOrWhiteSpace(token) ? token : _config.Token;
            if (!string.IsNullOrWhiteSpace(jwt))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Remove(header.Key);
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            if (method == HttpMethodType.Post || method == HttpMethodType.Put || method == HttpMethodType.Patch)
            {
                request.Content = BuildContent(data);
            }

            return request;
        }

        private static HttpMethod ToHttpMethod(HttpMethodType method)
        {
            switch (method)
            {
                case HttpMethodType.Get: return HttpMethod.Get;
                case HttpMethodType.Post: return HttpMethod.Post;
                case HttpMethodType.Put: return HttpMethod.Put;
                case HttpMethodType.Patch: return new HttpMethod("PATCH");
                case HttpMethodType.Delete: return HttpMethod.Delete;
                default: throw new NotSupportedException($"HTTP method '{method}' supported nahi hai.");
            }
        }

        private async Task ReadResponseBodyAsync<TResult>(HttpResponseMessage response, ApiResponse<TResult> result,
            CancellationToken cancellationToken)
        {
            if (response.Content == null || response.Content.Headers.ContentLength == 0)
                return;

            // Stream se directly deserialize karte hain - ReadAsStringAsync + Deserialize se
            // (double buffering) zyada memory-efficient aur fast, khaaskar bade payloads pe.
            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (stream == null || (stream.CanSeek && stream.Length == 0))
                    return;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using (var streamReader = new StreamReader(stream))
                        using (var jsonReader = new JsonTextReader(streamReader))
                        {
                            var serializer = JsonSerializer.Create(_serializerSettings);
                            result.Data = serializer.Deserialize<TResult>(jsonReader);
                        }
                    }
                    catch (JsonException ex)
                    {
                        // Response JSON shape expected TResult se match nahi hui.
                        result.ErrorDetails = $"Response could not be parsed as {typeof(TResult).Name}: {ex.Message}";
                    }
                }
                else
                {
                    // Error body chhota hota hai generally - poora text le lete hain diagnostics ke liye.
                    using (var streamReader = new StreamReader(stream))
                    {
                        result.ErrorDetails = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
            }
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
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint khaali nahi ho sakta.", nameof(endpoint));

            // Agar caller ne pehle se full absolute URL de diya (kisi doosre host/API ke liye),
            // usse BaseUrl ke saath jodne ki koshish nahi karte - yahi "accuracy" bug tha.
            if (Uri.TryCreate(endpoint, UriKind.Absolute, out var absoluteUri))
                return absoluteUri;

            var baseUrl = _config.BaseUrl.EndsWith("/") ? _config.BaseUrl : _config.BaseUrl + "/";
            var relative = endpoint.TrimStart('/');
            return new Uri(baseUrl + relative);
        }

        /// <summary>
        /// HttpClient process-wide cache me hai (best practice), isliye Dispose yahan client ko
        /// nahi todta - sirf interface compliance ke liye hai. Poore app lifecycle me isse
        /// dispose karne ki zarurat nahi.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
