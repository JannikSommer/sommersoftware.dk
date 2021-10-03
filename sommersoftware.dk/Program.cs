using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

namespace sommersoftware.dk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var clientInfo = ClientInfo.Load();

            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                //webBuilder.UseUrls("http://*:5000;https://*:5001;");
                webBuilder.UseStartup<Startup>()
                .ConfigureServices(services => services.AddSingleton(clientInfo));
            });
        }
    }
}
