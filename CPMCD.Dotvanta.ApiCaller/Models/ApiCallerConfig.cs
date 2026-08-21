using System;
using System.Collections.Generic;

namespace CPMCD.Dotvanta.ApiCaller.Models
{
    /// <summary>
    /// Universal config - Maui, Xamarin, Windows, WebApp sab isi ek object se
    /// apna BaseUrl / Token / Headers set karke ApiCaller ko de sakte hain.
    /// App khud iski instance bana kar DI mein register karega (singleton recommended
    /// taaki andar ka pooled HttpClient reuse ho, connections fresh na bane har call pe).
    /// </summary>
    public class ApiCallerConfig
    {
        /// <summary>Base endpoint, e.g. "https://myserver.com/api/"</summary>
        public string BaseUrl { get; set; }

        /// <summary>Bearer token (JWT). Optional - per-call bhi override ho sakta hai.</summary>
        public string Token { get; set; }

        /// <summary>Request timeout in minutes.</summary>
        public int TimeoutMinutes { get; set; } = 5;

        /// <summary>e.g. "android", "ios", "windows", "web" - custom header ke liye. Har platform khud apna bhejega.</summary>
        public string ApplicationType { get; set; } = "web";

        /// <summary>Extra static headers jo har request pe jayenge.</summary>
        public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Dev/self-signed cert environments ke liye. Production mein false rakhna.
        /// </summary>
        public bool BypassSslValidation { get; set; } = false;

        /// <summary>
        /// Transient failures (timeout, 5xx, connection reset) pe automatic retry karega.
        /// Speed ke liye default off — jahan zaruri ho wahi on karo.
        /// </summary>
        public bool EnableAutoRetry { get; set; } = false;

        /// <summary>Max retry attempts jab EnableAutoRetry true ho.</summary>
        public int MaxRetryCount { get; set; } = 2;

        /// <summary>Base delay (ms) between retries - exponential backoff use hota hai (delay, delay*2, delay*4...).</summary>
        public int RetryBaseDelayMilliseconds { get; set; } = 250;

        /// <summary>
        /// Kitni der tak ek connection pool me pooled rahe (DNS change pick karne ke liye).
        /// Long-running apps (mobile/desktop) me useful. Default 10 minutes.
        /// </summary>
        public TimeSpan PooledConnectionLifetime { get; set; } = TimeSpan.FromMinutes(10);
    }
}
