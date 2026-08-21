# CPMCD.Dotvanta.ApiCaller

A lightweight, platform-universal, high-performance HTTP API caller for .NET applications.

`CPMCD.Dotvanta.ApiCaller` provides a simple strongly typed abstraction for calling REST APIs from applications such as:

- .NET MAUI
- Xamarin
- Android/Windows .NET applications
- WPF / WinForms
- ASP.NET Core applications
- Other .NET clients compatible with `netstandard2.0`

## What's new in 1.0.1

- **Pooled `HttpClient` reuse** — the old version created a new `HttpClient`/`HttpClientHandler` (and therefore a new socket) on *every single call*. That's the classic .NET perf bug (socket exhaustion + slow DNS/TLS handshakes on every request). v2.0 caches one `HttpClient` per `BaseUrl`/settings combination for the lifetime of the app, so every platform (Android, iOS, Windows, Web) gets fast, connection-reused requests automatically via the runtime's native handler.
- **Streaming JSON (de)serialization** — responses are deserialized straight from the network stream instead of first buffering the whole body into a string and then parsing it. Lower memory, faster on large payloads.
- **Absolute URL fix** — passing a full URL as `endpoint` (instead of a relative path) used to get mangled by BaseUrl concatenation. Now `GetAsync("https://other-host/api/x")` is used as-is.
- **CancellationToken support** on every method — cancel a request from your app (page navigation away, user cancel button) without waiting for the timeout.
- **Automatic retry (opt-in)** — transient failures (`408`, `429`, `5xx`, timeouts) can be retried with exponential backoff via `EnableAutoRetry`. Client errors (`4xx` other than 408/429) are never retried.
- **Timeout vs. cancellation are now distinguishable** — `ApiResponse.IsTimeout` and `ApiResponse.IsCancelled` tell you exactly what happened instead of a generic exception message.
- **`ElapsedMilliseconds`** on every response for quick perf diagnostics/logging.
- **`PATCH` support** added alongside GET/POST/PUT/DELETE.
- **Per-call custom headers** in addition to per-call token override.
- Response gzip/deflate decompression enabled by default.

## Features

- GET / POST / PUT / PATCH / DELETE support
- Generic request and response models
- JSON serialization/deserialization using Newtonsoft.Json (streamed, not double-buffered)
- Bearer/JWT token support, with per-request override
- Per-request custom headers
- Default headers + application type header
- Configurable timeout
- CancellationToken support on every call
- Opt-in automatic retry with exponential backoff for transient failures
- Optional development SSL certificate bypass
- Platform-independent implementation — no MAUI/Xamarin dependency, pooled `HttpClient` works the same everywhere

## Installation

```bash
dotnet add package CPMCD.Dotvanta.ApiCaller
```

## Basic Configuration

```csharp
using CPMCD.Dotvanta.ApiCaller;
using CPMCD.Dotvanta.ApiCaller.Models;

var config = new ApiCallerConfig
{
    BaseUrl = "https://api.example.com/api/",
    TimeoutMinutes = 5,
    ApplicationType = "android"
};

var api = new ApiCallerService(config);
```

Register `ApiCallerConfig`/`ApiCallerService` as a **singleton** in DI wherever possible — that's what lets the pooled `HttpClient` actually get reused across your whole app.

## Configuration Options

```csharp
var config = new ApiCallerConfig
{
    BaseUrl = "https://api.example.com/api/",
    Token = "your-jwt-token",
    TimeoutMinutes = 5,
    ApplicationType = "android",
    DefaultHeaders = new Dictionary<string, string>
    {
        ["X-App-Version"] = "1.0.0",
        ["X-Client"] = "Dotvanta"
    },
    BypassSslValidation = false,
    EnableAutoRetry = true,
    MaxRetryCount = 2,
    RetryBaseDelayMilliseconds = 250
};
```

### Security Warning

Keep `BypassSslValidation = false` in production. It should only be used in controlled development / self-signed certificate environments.

## GET

```csharp
var response = await api.GetAsync<UserDto>("users/10");

if (response.IsSuccess)
{
    var user = response.Data;
}
else if (response.IsTimeout)
{
    Console.WriteLine("Server took too long to respond.");
}
else
{
    Console.WriteLine(response.ErrorDetails);
}
```

## POST / PUT / PATCH

```csharp
var response = await api.PostAsync<CreateUserRequest, UserDto>("users", request);
var updated  = await api.PutAsync<UpdateUserRequest, UserDto>("users/10", request);
var patched  = await api.PatchAsync<UpdateUserRequest, UserDto>("users/10", partialRequest);
```

## DELETE

```csharp
var response = await api.DeleteAsync<object>("users/10");
```

## Cancellation

```csharp
using var cts = new CancellationTokenSource();

var response = await api.GetAsync<UserDto>("users/10", cancellationToken: cts.Token);

if (response.IsCancelled)
{
    // caller cancelled it explicitly, not a server-side timeout
}
```

## Absolute URLs

`endpoint` can be a relative path (joined to `BaseUrl`) or a full absolute URL, which is used exactly as given:

```csharp
await api.GetAsync<WeatherDto>("https://other-service.example.com/weather"); // BaseUrl ignored
await api.GetAsync<UserDto>("users/10"); // joined to BaseUrl
```

## Per-call headers and token override

```csharp
var response = await api.GetAsync<UserDto>(
    "users/me",
    token: freshToken,
    headers: new Dictionary<string, string> { ["X-Correlation-Id"] = correlationId });
```

The per-request token always takes precedence over the configured one.

## Response Properties

```csharp
response.IsSuccess
response.StatusCode
response.Message
response.Data
response.ErrorDetails
response.ElapsedMilliseconds
response.IsTimeout
response.IsCancelled
```

## Dependency Injection

```csharp
builder.Services.AddSingleton(new ApiCallerConfig
{
    BaseUrl = "https://api.example.com/"
});
builder.Services.AddSingleton<IApiCaller, ApiCallerService>();
```

Then inject `IApiCaller` wherever needed.

## Supported Target

```text
netstandard2.0
```

## Dependency

```text
Newtonsoft.Json 13.0.3
```

## Recommended Production Practices

- Register `ApiCallerService` as a singleton (or reuse one instance) so the pooled `HttpClient` is actually shared.
- Do not hardcode JWT tokens.
- Do not enable SSL validation bypass in production.
- Store API base URLs in environment/configuration settings.
- Use short-lived access tokens where appropriate.
- Handle `401`, `403`, `429`, `5xx` responses explicitly in application code.
- Turn on `EnableAutoRetry` only for idempotent calls (GET, PUT, DELETE) — be careful enabling it for POST unless the endpoint is idempotent.
- Do not log access tokens or sensitive request/response data.

## Build

```bash
dotnet restore
dotnet build -c Release
dotnet pack -c Release
```

## Author

**CPMCD : Faisal Raza Khan**

## License

Add the final project license before public distribution.
