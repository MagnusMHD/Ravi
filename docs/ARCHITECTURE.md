# Architecture

## Current shape

RAVI starts as a small offline-first student application. The solution deliberately separates mobile presentation from learning behavior.

```text
Ravi.App  ──────>  Ravi.Core
   │                  │
 .NET MAUI       Missions and steps
 XAML + MVVM     Progress rules
```

### `Ravi.App`

Owns pages, navigation, presentation state, localization, device integration, and packaged media. View models may orchestrate use cases but should not contain curriculum rules.

### `Ravi.Core`

Owns platform-independent concepts such as missions, ordered learning steps, completion, and progress. It has no MAUI or third-party package dependency.

### `Ravi.Core.Tests`

Contains package-free executable checks for the early prototype. This keeps the learning core verifiable in restricted environments. It can be migrated to a conventional test framework when the dependency policy is established.

## Planned boundaries

- `Content`: versioned curriculum mappings and story missions
- `Progress`: local attempts, mastery, and review scheduling
- `Persistence`: SQLite and secure preferences
- `Sync`: later server synchronization with idempotent operations
- `Localization`: Persian RTL shell and English LTR learning content

The first backend should be a modular ASP.NET Core application, introduced only when accounts or cross-device synchronization become part of the student MVP.
