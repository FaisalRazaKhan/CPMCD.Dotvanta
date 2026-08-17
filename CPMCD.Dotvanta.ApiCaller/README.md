# CPMCD.Dotvanta.ApiCaller

A lightweight, platform-independent HTTP API caller for .NET applications.

`CPMCD.Dotvanta.ApiCaller` provides a simple strongly typed abstraction for calling REST APIs from applications such as:

- .NET MAUI
- Xamarin
- Android/Windows .NET applications
- WPF / WinForms
- ASP.NET Core applications
- Other .NET clients compatible with `netstandard2.0`

## Features

- GET support
- POST support
- PUT support
- DELETE support
- Generic request and response models
- JSON serialization/deserialization using Newtonsoft.Json
- Bearer/JWT token support
- Per-request token override
- Default headers
- Application type header
- Configurable timeout
- Optional development SSL certificate bypass
- Platform-independent implementation
- No MAUI dependency

## Installation

```bash
dotnet add package CPMCD.Dotvanta.ApiCaller
```

Or install from the NuGet Package Manager:

```text
CPMCD.Dotvanta.ApiCaller
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
    BypassSslValidation = false
};
```

### Security Warning

Keep:

```csharp
BypassSslValidation = false;
```

in production.

SSL validation bypass should only be considered for controlled development/self-signed certificate environments.

## GET

```csharp
var response = await api.GetAsync<UserDto>("users/10");

if (response.IsSuccess)
{
    var user = response.Data;
}
else
{
    Console.WriteLine(response.ErrorDetails);
}
```

## POST

```csharp
var request = new CreateUserRequest
{
    Name = "Faisal",
    Email = "faisal@example.com"
};

var response =
    await api.PostAsync<CreateUserRequest, UserDto>(
        "users",
        request);

if (response.IsSuccess)
{
    var createdUser = response.Data;
}
```

## PUT

```csharp
var request = new UpdateUserRequest
{
    Name = "Faisal Updated"
};

var response =
    await api.PutAsync<UpdateUserRequest, UserDto>(
        "users/10",
        request);
```

## DELETE

```csharp
var response =
    await api.DeleteAsync<object>("users/10");

if (response.IsSuccess)
{
    Console.WriteLine("Deleted successfully.");
}
```

## JWT / Bearer Token

A default token can be configured:

```csharp
var config = new ApiCallerConfig
{
    BaseUrl = "https://api.example.com/",
    Token = accessToken
};
```

A token can also be supplied per request:

```csharp
var response =
    await api.GetAsync<UserDto>(
        "users/me",
        token);
```

The per-request token takes precedence over the configured token.

## Strongly Typed Responses

Every operation returns:

```csharp
ApiResponse<T>
```

Example:

```csharp
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

Then:

```csharp
ApiResponse<UserDto> response =
    await api.GetAsync<UserDto>("users/10");
```

## Response Properties

```csharp
response.IsSuccess
response.StatusCode
response.Message
response.Data
response.ErrorDetails
```

## Dependency Injection

The current package exposes `ApiCallerService` directly and does not require a DI framework.

For an ASP.NET Core or MAUI application, you can register it yourself:

```csharp
builder.Services.AddSingleton<IApiCaller>(_ =>
{
    var config = new ApiCallerConfig
    {
        BaseUrl = "https://api.example.com/"
    };

    return new ApiCallerService(config);
});
```

Then inject:

```csharp
public class UserService
{
    private readonly IApiCaller _api;

    public UserService(IApiCaller api)
    {
        _api = api;
    }
}
```

## Supported Target

```text
netstandard2.0
```

## Dependency

```text
Newtonsoft.Json 13.0.3
```

## Recommended Production Practices

- Do not hardcode JWT tokens.
- Do not enable SSL validation bypass in production.
- Store API base URLs in environment/configuration settings.
- Use short-lived access tokens where appropriate.
- Handle `401`, `403`, `429`, `5xx` responses explicitly in application code.
- Consider centralized retry/circuit-breaker handling for production APIs.
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
