using System;
using System.Collections.Generic;

namespace CPMCD.Dotvanta.ApiCaller.Models
{
    /// <summary>
    /// Universal config - Maui, Xamarin, Windows, WebApp sab isi ek object se
    /// apna BaseUrl / Token / Headers set karke ApiCaller ko de sakte hain.
    /// App khud iski instance bana kar DI mein register karega.
    /// </summary>
    public class ApiCallerConfig
    {
        /// <summary>Base endpoint, e.g. "https://myserver.com/api/"</summary>
        public string BaseUrl { get; set; }

        /// <summary>Bearer token (JWT). Optional - per-call bhi override ho sakta hai.</summary>
        public string Token { get; set; }

        /// <summary>Request timeout in minutes.</summary>
        public int TimeoutMinutes { get; set; } = 5;

        /// <summary>e.g. "android", "ios", "windows", "web" - custom header ke liye.</summary>
        public string ApplicationType { get; set; } = "web";

        /// <summary>Extra static headers jo har request pe jayenge.</summary>
        public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Dev/self-signed cert environments ke liye. Production mein false rakhna.
        /// </summary>
        public bool BypassSslValidation { get; set; } = false;
    }
}
