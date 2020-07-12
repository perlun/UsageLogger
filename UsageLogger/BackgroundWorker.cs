using Dapper;
using MySql.Data.MySqlClient;
using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Timers;

namespace UsageLogger
{
    public class BackgroundWorker
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

        private string ConnectionString
        {
            get => ConfigurationManager.ConnectionStrings["main"].ConnectionString;
        }

        private string InsertQuery
        {
            get => @"
                INSERT INTO log_entry (
                    host_name,
                    login_name,
                    process_name,
                    file_name,
                    duration
                )
                VALUES (
                    @HostName,
                    @LoginName,
                    @ProcessName,
                    @FileName,
                    @Duration
                )
            ";
        }

        /// <summary>
        /// Returns the host and login name, on this form: MYMACHINE\toor
        /// </summary>
        private string HostAndLoginName
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
                ManagementObjectCollection collection = searcher.Get();
                return (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            }
        }

        public void Run()
        {
            RunMigrations();

            timer.Elapsed += (sender, e) =>
            {
                var idleTime = IdleTimeFinder.GetIdleTime();
                if (idleTime > IDLE_TIMEOUT)
                {
                    Console.WriteLine($"User idle {idleTime} milliseconds, not logging");
                    return;
                }

                IntPtr handle = GetForegroundWindow();
                GetWindowThreadProcessId(handle, out uint processId);

                // QUIRK: Why does GetWindowThreadProcessId() expect a uint whereas
                // Process.GetProcessById expects a _signed_ integer?
                var process = Process.GetProcessById((int)processId);

                // The semantics here was derived purely from trial-and-error - I ran the program, leaving the machine in various
                // states which helped me deduce these "do not log" scenarios.
                if (process.Id == 0)
                {
                    // "Idle" process
                }
                else
                {
                    if (process.ProcessName == "LockApp")
                    {
                        // The Ctrl - Alt - Del lock screen.
                    }
                    else
                    {
                        Console.WriteLine($"{process.ProcessName} {process.MainModule.FileName}");

                        var list = HostAndLoginName.Split(new[] { '\\' });
                        var hostName = list[0];
                        var loginName = list[1];

                        using (var connection = new MySqlConnection(ConnectionString))
                        {
                            connection.Execute(InsertQuery, new
                            {
                                hostName,
                                loginName,
                                process.ProcessName,
                                process.MainModule.FileName,
                                Duration = TIMER_INTERVAL / 1000
                            });
                        }
                    }
                }
            };
            timer.Enabled = true;

            Console.WriteLine("Program initialized, waiting on timer");
        }

        private void RunMigrations()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var databaseProvider = new MysqlDatabaseProvider(connection);
                var migrationsAssembly = GetType().Assembly;
                var migrator = new SimpleMigrator(migrationsAssembly, databaseProvider);

                migrator.Load();
                migrator.MigrateToLatest();
            }
        }
    }
}
