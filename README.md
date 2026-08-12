# RAVI English

RAVI is a story-driven English-learning app for Iranian students in grades 7–9. The fox guide Ravi connects vocabulary, story, grammar, and listening in one motivating daily journey.

> **Status:** early student-only prototype. The included lesson is demonstration content and does not yet claim alignment with an official textbook.

## Experience

Every mission follows a predictable learning rhythm:

```text
Vocabulary → Story → Grammar → Listening → Reward
```

The interface is designed for mixed Persian RTL and English LTR content, short daily sessions, offline use, and supportive feedback without punitive mechanics.

## Repository structure

- `src/Ravi.App` — .NET MAUI student experience
- `src/Ravi.Core` — platform-independent mission and progress logic
- `tests/Ravi.Core.Tests` — package-free executable core checks
- `outputs` — approved mascot and brand concept artifacts
- `docs` — architecture and product roadmap

## Prerequisites

- .NET SDK 10.0.103 or a compatible patch
- .NET MAUI workloads for mobile builds
- Android SDK and/or Xcode for the selected target

## Verify the learning core

```sh
dotnet run --project tests/Ravi.Core.Tests/Ravi.Core.Tests.csproj
```

## Build the MAUI app

Install or restore the required workloads, then build the selected platform:

```sh
dotnet workload restore src/Ravi.App/Ravi.App.csproj
dotnet build src/Ravi.App/Ravi.App.csproj -f net10.0-android
```

## Documentation

- [Architecture](docs/ARCHITECTURE.md)
- [Roadmap](docs/ROADMAP.md)
- [Contributing](CONTRIBUTING.md)
- [Security policy](SECURITY.md)

## Brand assets

The repository includes the approved Ravi mascot sheet and the current logo concept. Generated source concepts are kept for traceability; production-ready vector and animation assets will follow.

## Licensing

No open-source license has been selected yet. Until one is added, all rights are reserved by the repository owner.
