using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elf.Pages
{
    public class ServerCheckModel : PageModel
    {
        [BindProperty]
        public string hostname { get; set; }

        [BindProperty]
        public short port { get; set; } = 443;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Console.WriteLine($"Hostname: {hostname}; Port: {port}");
            if(hostname.Trim().Length > 0 && port > 0 && port <= 65535)
            {
                var hostEntry = Dns.GetHostEntry(hostname);

                var ipv4Address = hostEntry.AddressList.First(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                new ServerCheck(ipv4Address, port, hostname, false).run();
                new ServerCheck(ipv4Address, port, hostname, true).run();

                var ipv6Address = hostEntry.AddressList.First(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
                new ServerCheck(ipv6Address, port, hostname, false).run();
                new ServerCheck(ipv6Address, port, hostname, true).run();
            }

            return RedirectToPage("ServerCheck");
        }
    }
}
