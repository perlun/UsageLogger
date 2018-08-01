using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;

namespace UsageLoggerService
{
    public class Service : ServiceBase
    {
#if DEBUG
        const int TIMER_INTERVAL = 3_000;
        const int IDLE_TIMEOUT = 3_000;
#else
        const int TIMER_INTERVAL = 60_000;
        const int IDLE_TIMEOUT = 60_000;
#endif

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private readonly Timer timer = new Timer
        {
            AutoReset = true,
            Interval = TIMER_INTERVAL
        };

        public Service()
        {
            this.ServiceName = "UsageLoggerService";
        }

        protected override void OnStart(string[] args)
        {
            Run();
        }

        public void Run()
        {
            timer.Elapsed += (sender, e) =>
            {
                if (IdleTimeFinder.GetIdleTime() > IDLE_TIMEOUT)
                {
                    Console.WriteLine("User idle, not logging");
                    return;
                }

                IntPtr handle = GetForegroundWindow();
                GetWindowThreadProcessId(handle, out uint processId);

                // QUIRK: Why does GetWindowThreadProcessId() expect a uint whereas
                // Process.GetProcessById expects a _signed_ integer?
                var process = Process.GetProcessById((int)processId);

                if (process.Id == 0)
                {
                    // "Idle" process
                }
                else
                {
                    if (process.ProcessName == "LockApp")
                    {
                        // Likely Ctrl - Alt - Del lock screen.
                    }
                    else
                    {
                        // TODO: Log to database instead of just writing to the console.
                        Console.WriteLine($"{process.ProcessName} {process.MainModule.FileName}");
                    }
                }
            };
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
        }
    }
}
