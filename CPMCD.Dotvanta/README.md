# CPMCD.Dotvanta

## Overview

`CPMCD.Dotvanta` is the foundation package in the Dotvanta library family.

It provides the base namespace/package identity for the CPMCD.Dotvanta ecosystem and is intended to host shared, platform-independent functionality as the library family grows.

## Target Framework

```text
netstandard2.0
```

This target is intentionally broad so shared code can be consumed by multiple .NET application types.

## Current Scope

The current project is a foundation package and does not expose a significant public API yet.

The reusable functionality currently lives in the sibling packages:

- `CPMCD.Dotvanta.ApiCaller`
- `CPMCD.Dotvanta.Component`
- `CPMCD.Dotvanta.Mail`

## Installation

When published to NuGet:

```bash
dotnet add package CPMCD.Dotvanta
```

## When Should I Use This Package?

Use this package when your application specifically depends on the common Dotvanta foundation package.

For actual functionality, install the package that matches your requirement:

```bash
dotnet add package CPMCD.Dotvanta.ApiCaller
dotnet add package CPMCD.Dotvanta.Component
dotnet add package CPMCD.Dotvanta.Mail
```

## Development

Build:

```bash
dotnet build -c Release
```

Pack:

```bash
dotnet pack -c Release
```

## Roadmap

Potential future shared functionality:

- Common result types
- Shared validation helpers
- Common extensions
- Cross-platform utility abstractions
- Shared constants and contracts

## Author

**CPMCD : Faisal Raza Khan**

## License

Add the final project license before public NuGet distribution.
