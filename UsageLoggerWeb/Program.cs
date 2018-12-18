using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace UsageLoggerWeb
{
    public class Program
    {
        public static string ConnectionString { get; set; }

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls(urls: "http://0.0.0.0:5000")
                .UseStartup<Startup>();
    }
}
