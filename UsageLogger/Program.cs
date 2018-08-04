#if DEBUG
using System.Diagnostics;
#endif

using System.Runtime.InteropServices;
using System.Threading;

namespace UsageLogger
{
    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        static int Main(string[] args)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                // Make it possible to see what the program is doing by opening up a console window.
                AllocConsole();
            }
#endif

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.Run();

            // Put a breakpoint on the following line to always catch the service when it has finished its work
            Thread.Sleep(Timeout.Infinite);

            return 0;
        }
    }
}
