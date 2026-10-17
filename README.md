# TempLock.Examples

Runnable usage examples for **[TempLock](https://www.nuget.org/packages/TempLock)** — a hardened
trial/evaluation licensing library for Windows desktop applications.

> This repository shows how to **use** the library. It does not describe internal behaviour or
> storage locations. All examples consume the published NuGet package.

## What is TempLock?

TempLock enforces a one-time, time-limited trial (X days from first registration) and is hardened
against the usual circumvention attempts: setting the clock back, deleting the stored state,
reinstalling the app, and running inside a virtual machine. State is machine-bound and stored
encrypted; no server is required.

- Target frameworks: `net48`, `net8.0-windows`, `net10.0-windows` (Windows only)
- Per-user or per-machine trials
- Clock-rollback detection with a configurable grace window
- Tamper and reinstall protection
- Configurable virtual-machine policy (Block / Warn / Allow)
- No telemetry; the only optional network traffic is a standard NTP time query

## Install

```
dotnet add package TempLock
```

## Minimal usage

```csharp
using TempLock;

var trial = new TempLockManager(new TempLockOptions
{
    ApplicationId = "com.example.myapp",   // stable identifier, never change it
    ApplicationSecret = MyEmbeddedSecret,  // byte[] baked into your app
    TrialDays = 30,
    Scope = TrialScope.PerUser,            // or PerMachine
    VmPolicy = VmPolicy.Warn,              // Block | Warn | Allow (Warn is the default)
    EnableNtpCheck = true
});

TrialStatus status = trial.EnsureRegistered();

if (!status.IsValid)
{
    // status.State is Expired / ClockRolledBack / Tampered / VmBlocked
    // show a message and exit or limit functionality
}
```

Call `EnsureRegistered()` once at application start. Use `GetStatus()` to re-check without ever
starting a new trial.

## Examples in this repository

| Example | What it shows |
|---------|---------------|
| [`TempLock.Demo.Console`](TempLock.Demo.Console) | Minimal console integration: register on start, evaluate `TrialStatus`. |
| [`TempLock.Demo.Wpf`](TempLock.Demo.Wpf) | WPF app with a trial banner, days-remaining display and locked-state handling. |
| [`installer/setup.iss`](installer/setup.iss) | Inno Setup installer that seeds a per-machine trial elevated and keeps it on uninstall. |

### Build & run

```
dotnet build TempLock.Samples.sln -c Release
dotnet run --project TempLock.Demo.Console -c Release
```

Requires the .NET SDK on Windows. The examples restore `TempLock` from NuGet.

## Per-machine trials

For `TrialScope.PerMachine`, seed the machine-wide state from the (elevated) installer once — see
`installer/setup.iss`. When a lower-privileged user runs the app afterwards, TempLock mirrors the
state into the user-writable locations automatically during the early part of the trial.

## Legal / privacy

Trial enforcement processes device-bound data on the local machine. Disclose trial tracking in your
EULA / privacy policy. The library sends no user data; the optional NTP check is a plain time query.

## License

The TempLock library is proprietary. See the package for its license terms. The example code in this
repository may be used freely as a starting point.
