# TempLock.Examples

Usage examples for **[TempLock](https://www.nuget.org/packages/TempLock)** — a hardened
trial/evaluation licensing library for Windows desktop applications.

> This repository will hold runnable examples. It documents how to **use** the library only; it does
> not describe internal behaviour or storage locations.

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
    VmPolicy = VmPolicy.Block,             // Block | Warn | Allow
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

## Legal / privacy

Trial enforcement processes device-bound data on the local machine. Disclose trial tracking in your
EULA / privacy policy. The library sends no user data; the optional NTP check is a plain time query.

## License

The TempLock library is proprietary. See the package for its license terms.
