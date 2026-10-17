using System;
using TempLock;

namespace TempLock.Demo.Console
{
    internal static class Program
    {
        // In a real app, embed a non-trivial secret and keep it out of public source.
        private static readonly byte[] AppSecret =
            { 0x2A, 0x9F, 0x11, 0x74, 0xC3, 0x08, 0x55, 0xEE, 0x6B, 0x19, 0xA0, 0x4D };

        private static int Main()
        {
            var options = new TempLockOptions
            {
                ApplicationId = "com.example.templock.democonsole",
                ApplicationSecret = AppSecret,
                TrialDays = 30,
                Scope = TrialScope.PerUser,
                VmPolicy = VmPolicy.Warn,     // demo: don't hard-block so it runs in CI/VMs
                EnableNtpCheck = true
            };

            var trial = new TempLockManager(options);
            TrialStatus status = trial.EnsureRegistered();

            System.Console.WriteLine("TempLock demo");
            System.Console.WriteLine("  State:          " + status.State);
            System.Console.WriteLine("  Days remaining: " + status.DaysRemaining);
            System.Console.WriteLine("  Started (UTC):  " + status.StartedUtc);
            System.Console.WriteLine("  Expires (UTC):  " + status.ExpiresUtc);
            System.Console.WriteLine("  Virtual machine:" + status.IsVirtualMachine);

            switch (status.State)
            {
                case TrialState.Active:
                    System.Console.WriteLine();
                    System.Console.WriteLine("Trial is active. Full functionality available.");
                    return 0;

                case TrialState.Expired:
                    System.Console.WriteLine();
                    System.Console.WriteLine("Trial period has ended. Please purchase a license.");
                    return 1;

                case TrialState.ClockRolledBack:
                    System.Console.WriteLine();
                    System.Console.WriteLine("The system clock was moved backwards. Trial is locked.");
                    return 2;

                case TrialState.Tampered:
                    System.Console.WriteLine();
                    System.Console.WriteLine("Trial data is inconsistent. Trial is locked.");
                    return 3;

                case TrialState.VmBlocked:
                    System.Console.WriteLine();
                    System.Console.WriteLine("Running inside a virtual machine is not allowed.");
                    return 4;

                default:
                    return 5;
            }
        }
    }
}
