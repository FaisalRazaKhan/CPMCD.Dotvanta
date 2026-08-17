# CPMCD.Dotvanta

A reusable .NET ecosystem library collection by **CPMCD** for building modern applications with .NET MAUI, ASP.NET Core, and other .NET clients.

## Packages

| Package | Purpose | Target |
|---|---|---|
| `CPMCD.Dotvanta` | Foundation package / namespace for the Dotvanta ecosystem | `netstandard2.0` |
| `CPMCD.Dotvanta.ApiCaller` | Generic HTTP API client with GET/POST/PUT/DELETE, JWT, headers and typed responses | `netstandard2.0` |
| `CPMCD.Dotvanta.Component` | Reusable .NET MAUI XAML controls and UI helpers | `net9.0-android` |
| `CPMCD.Dotvanta.Mail` | Reusable SMTP mail service with DI, configuration and attachments | `net9.0` |

## Repository Structure

```text
CPMCD.Dotvanta/
├── CPMCD.Dotvanta/
├── CPMCD.Dotvanta.ApiCaller/
├── CPMCD.Dotvanta.Component/
└── CPMCD.Dotvanta.Mail/
```

## Design Goals

- Reusable NuGet packages
- Minimal platform-specific dependencies
- Strongly typed APIs
- Dependency Injection friendly services
- Production-oriented configuration
- Secure-by-default configuration where practical
- Clean separation between reusable libraries and consuming applications

## NuGet Packages

The individual libraries can be published independently:

```bash
dotnet pack CPMCD.Dotvanta.ApiCaller/CPMCD.Dotvanta.ApiCaller.csproj -c Release
dotnet pack CPMCD.Dotvanta.Component/CPMCD.Dotvanta.Component.csproj -c Release
dotnet pack CPMCD.Dotvanta.Mail/CPMCD.Dotvanta.Mail.csproj -c Release
```

## Repository / Package Naming

The recommended package naming convention is:

```text
CPMCD.Dotvanta.*
```

This keeps all libraries grouped under the same product family.

## License

Add the project's final license before publishing to NuGet. For public distribution, also consider adding:

- `LICENSE`
- `CHANGELOG.md`
- `CONTRIBUTING.md`
- `SECURITY.md`

## Author

**CPMCD : Faisal Raza Khan**

## Status

This repository is under active development. APIs may change between early releases. Use semantic versioning for future releases.

---
© CPMCD. All rights reserved unless a separate project license states otherwise.
