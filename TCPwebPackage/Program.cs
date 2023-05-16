using System;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace TCPwebPackage
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Robot robot = new Robot();
            Data data = new Data();

            Console.WriteLine("Write a url to website you want crawl!");
            var userUrl = Console.ReadLine();

            (GlobalVariables.Url, string useless) = data.FormatRequests(userUrl);

            (string domain, string page) = data.FormatRequests(userUrl);
          

            string response = await data.Get(domain,page);

            Console.WriteLine("Crawl the page? Y | N");
            var botcrawl = Console.ReadLine();

            if (botcrawl == "y")
            {
                await robot.Crawl(response);
            }

            //await data.Write(response);
        }
    }
}