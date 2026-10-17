; Inno Setup example for an app protected with TempLock.
;
; Key points demonstrated:
;  * The installer runs elevated, so it can seed the machine-wide trial locations once
;    (TrialScope.PerMachine). A lower-privileged user running the app later is handled gracefully by
;    TempLock during the first 1/8 of the trial.
;  * The uninstaller intentionally does NOT remove the trial state, so reinstalling does not reset
;    the trial.
;
; This assumes your application implements a "/InitTrial" switch that constructs a TempLockManager
; with Scope = PerMachine and calls EnsureRegistered(), then exits.

#define AppName "My Protected App"
#define AppVersion "1.0.0"
#define AppExe "MyProtectedApp.exe"
#define AppPublisher "Volker Hänsel"

[Setup]
AppId={{B7F3B9F1-7C2E-4C0C-9E2E-9B9A2C1D0A11}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
PrivilegesRequired=admin
OutputBaseFilename=MyProtectedApp-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern

[Files]
Source: "dist\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{group}\Uninstall {#AppName}"; Filename: "{uninstallexe}"

[Run]
; Seed the per-machine trial locations once, elevated, right after install.
Filename: "{app}\{#AppExe}"; Parameters: "/InitTrial"; Flags: runhidden waituntilterminated; \
  StatusMsg: "Initializing trial..."
; Offer to launch the app after setup.
Filename: "{app}\{#AppExe}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; Nothing here removes the trial state on purpose. Reinstalling must not reset the trial.

; [UninstallDelete] is deliberately omitted for the TempLock storage locations.
