using System;
using DC.Web.Authorization.Data;
using DC.Web.Authorization.Data.SeedData;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DC.Web.Ui
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
    }
}