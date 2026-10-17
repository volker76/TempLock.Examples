using System;
using System.Windows;
using System.Windows.Media;
using TempLock;

namespace TempLock.Demo.Wpf
{
    public partial class MainWindow : Window
    {
        private static readonly byte[] AppSecret =
            { 0x7C, 0x51, 0xF2, 0x03, 0x88, 0x2D, 0x9A, 0x40, 0x1E, 0xBB, 0x64, 0x05 };

        private TrialState _state;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var options = new TempLockOptions
            {
                ApplicationId = "com.example.templock.demowpf",
                ApplicationSecret = AppSecret,
                TrialDays = 30,
                Scope = TrialScope.PerUser,
                VmPolicy = VmPolicy.Warn,
                EnableNtpCheck = true
            };

            TrialStatus status;
            try
            {
                status = new TempLockManager(options).EnsureRegistered();
            }
            catch (Exception ex)
            {
                HeaderText.Text = "Trial check failed";
                DetailText.Text = ex.Message;
                return;
            }

            _state = status.State;
            Render(status);
        }

        private void Render(TrialStatus status)
        {
            DetailText.Text =
                $"State: {status.State}\n" +
                $"Days remaining: {status.DaysRemaining}\n" +
                $"Expires (UTC): {status.ExpiresUtc:yyyy-MM-dd HH:mm}\n" +
                $"Virtual machine: {status.IsVirtualMachine}";

            switch (status.State)
            {
                case TrialState.Active:
                    HeaderText.Text = $"Trial active — {status.DaysRemaining} day(s) left";
                    MessageText.Text = "Full functionality is available during the trial period.";
                    Banner.Background = new SolidColorBrush(Color.FromRgb(0xEE, 0xF7, 0xEE));
                    ActionButton.Content = "Continue";
                    break;

                case TrialState.Expired:
                    HeaderText.Text = "Trial expired";
                    MessageText.Text = "Your evaluation period has ended. Please purchase a license.";
                    Banner.Background = new SolidColorBrush(Color.FromRgb(0xFB, 0xEA, 0xEA));
                    ActionButton.Content = "Close";
                    break;

                case TrialState.ClockRolledBack:
                    HeaderText.Text = "Trial locked";
                    MessageText.Text = "The system clock was moved backwards. The trial is locked.";
                    Banner.Background = new SolidColorBrush(Color.FromRgb(0xFB, 0xEA, 0xEA));
                    ActionButton.Content = "Close";
                    break;

                case TrialState.Tampered:
                    HeaderText.Text = "Trial locked";
                    MessageText.Text = "Trial data is inconsistent and the trial has been locked.";
                    Banner.Background = new SolidColorBrush(Color.FromRgb(0xFB, 0xEA, 0xEA));
                    ActionButton.Content = "Close";
                    break;

                case TrialState.VmBlocked:
                    HeaderText.Text = "Not allowed in a VM";
                    MessageText.Text = "This application cannot run inside a virtual machine.";
                    Banner.Background = new SolidColorBrush(Color.FromRgb(0xFB, 0xEA, 0xEA));
                    ActionButton.Content = "Close";
                    break;
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_state != TrialState.Active)
                Close();
            else
                MessageBox.Show("Launching the app...", "TempLock Demo");
        }
    }
}
