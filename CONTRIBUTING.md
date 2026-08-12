# Contributing to RAVI English

Thank you for helping build RAVI. Keep changes focused, student-safe, accessible, and easy to review.

## Development flow

1. Create a short-lived branch from `main`.
2. Keep curriculum content separate from presentation and navigation code.
3. Add or update core checks when changing mission behavior.
4. Run the core verification locally.
5. Open a pull request describing the student impact and validation performed.

```sh
dotnet build src/Ravi.Core/Ravi.Core.csproj
dotnet run --project tests/Ravi.Core.Tests/Ravi.Core.Tests.csproj
```

Mobile builds require the appropriate .NET MAUI workloads and platform SDKs.

## Product principles

- Design for students aged 11–14.
- Treat mistakes as learning signals, never as punishment.
- Keep Persian RTL and English LTR content readable together.
- Do not add tracking, advertising, open chat, or personal-data collection without an explicit privacy and safety review.
- Do not claim alignment with a textbook until the content mapping has been reviewed.

## Content and assets

Only commit content and media that the project is allowed to use. Large production media should eventually move to an appropriate asset pipeline rather than growing the Git history indefinitely.
