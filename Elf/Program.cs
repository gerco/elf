using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elf
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CreateHostBuilder(args).Build().Run();

            var hostname = "home.dries.info";
            short port = 443;

            var hostEntry = Dns.GetHostEntry(hostname);

            var ipv4Address = hostEntry.AddressList.First(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            new ServerCheck(ipv4Address, port, hostname, false).run();
            new ServerCheck(ipv4Address, port, hostname, true).run();

            var ipv6Address = hostEntry.AddressList.First(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
            new ServerCheck(ipv6Address, port, hostname, false).run();
            new ServerCheck(ipv6Address, port, hostname, true).run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
