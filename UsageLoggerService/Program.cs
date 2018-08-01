using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace UsageLoggerService
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                // Debug code: this allows the process to run as a non-service. It will kick off the service start point, but
                // never kill it. Shut down the debugger to exit the process.
                var service = new Service();
                service.Run();

                // Put a breakpoint on the following line to always catch the service when it has finished its work
                Thread.Sleep(Timeout.Infinite);
            }
            else if (Environment.UserInteractive)
            {
                if (args.Length != 1)
                {
                    DisplayUsage();
                    return -1;
                }

                var firstArg = args[0];

                switch (firstArg)
                {
                    case "/i":
                    case "/install":
                        return InstallService();

                    case "/u":
                    case "/uninstall":
                        return UninstallService();

                    default:
                        Console.WriteLine("Argument not recognized: {0}", args[0]);
                        Console.WriteLine(string.Empty);
                        DisplayUsage();
                        return 1;
                }
            }
            else
            {

                var servicesToRun = new ServiceBase[] { new Service() };
                ServiceBase.Run(servicesToRun);
            }

            return 0;
        }

        private static int InstallService()
        {
            var service = new Service();

            try
            {
                var args = new List<string>
                {
                    Assembly.GetExecutingAssembly().Location
                };

                // Install the service with the Windows Service Control Manager (SCM)
                ManagedInstallerClass.InstallHelper(args.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine();
                Console.WriteLine("InvalidOperationException: This can be caused when the service isn't already installed.");
                Console.WriteLine();
                Console.WriteLine(ex.ToString());
                return -1;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(Win32Exception))
                {
                    var wex = (Win32Exception)ex.InnerException;
                    Console.WriteLine("Error(0x{0:X}): Service already installed!", wex.ErrorCode);
                    return wex.ErrorCode;
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }

            return 0;
        }

        private static int UninstallService()
        {
            try
            {
                var args = new List<string>
                {
                    "/u",
                    Assembly.GetExecutingAssembly().Location
                };

                // uninstall the service from the Windows Service Control Manager (SCM)
                ManagedInstallerClass.InstallHelper(args.ToArray());
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine();
                Console.WriteLine("InvalidOperationException: This can be caused when the service is not previously installed.");
                Console.WriteLine();
                Console.WriteLine(ex.ToString());
                return -1;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.GetType() == typeof(Win32Exception))
                {
                    Win32Exception wex = (Win32Exception)ex.InnerException;
                    Console.WriteLine("Error(0x{0:X}): Service not installed!", wex.ErrorCode);
                    return wex.ErrorCode;
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }

            return 0;
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("Usage: UsageLoggerService /i[nstall] | /u[ninstall]");
        }
    }
}
