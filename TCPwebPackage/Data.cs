using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.Pipes;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
namespace TCPwebPackage
{
    internal class Data
    {

        public async Task<string> Get(string domain, string page = "")
        {
            Stopwatch stopwatch = new Stopwatch();
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    await client.ConnectAsync(domain, 443);
                    Console.WriteLine("\nConnected!\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nSite doesn't exist!\n");
                    Console.WriteLine(ex.ToString());
                    return "Site doesn't exist";
                }
                using (var stream = new NetworkStream(client))
                {
                    using (var sslStream = new SslStream(stream))
                    {
                        try
                        {
                            sslStream.AuthenticateAsClient(domain);
                        }catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            return "Couldn't authenticate as client";
                        }
                        string request;

                        if (page == "")
                        {
                            request = $"GET / HTTP/1.1\r\nHost: {domain}\r\nConnection: close\r\nUser-Agent: Client/1.0\r\n\r\n";
                        }
                        else
                        {
                            request = $"GET {page} HTTP/1.1\r\nHost: {domain}\r\nConnection: close\r\nUser-Agent: Client/1.0\r\n\r\n";
                        }

                        byte[] requestBytes = Encoding.UTF8.GetBytes(request);
                        byte[] buffer = new byte[2048];
                        string path = @"E:\tescik\tempfile.txt";

                        stopwatch.Start();
                        bool isTimeExceeded = false;

                        await sslStream.WriteAsync(requestBytes, 0, requestBytes.Length);

                        using (var reader = new StreamReader(sslStream))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
            }
        }
        public async Task Write(string data)
        {
            Console.Clear();

            Console.WriteLine("Where do you want to save it?");
            var userpath = Console.ReadLine();
            string path = $@"{userpath}";
            Console.Clear();

            Console.WriteLine("HTML only? Y | N");
            var html = Console.ReadLine();
            html = html.ToLower();

            Console.Clear();

            if (html == "y")
            {
                string htmlDeclaration = "<!";
                if (data.Contains(htmlDeclaration))
                {
                    string[] dataSplit = data.Split(htmlDeclaration, StringSplitOptions.None);
                    data = "<!" + dataSplit[1];
                }
                else
                {
                    Console.WriteLine("No HTML Declaration, Check for mistakes in URL you provided\n");
                    Console.WriteLine($"ERROR MESSAGE:\n\n{data}");
                }
            }

            Process[] processes = Process.GetProcessesByName("explorer");

            if (processes.Length > 0)
            {
                // Terminate the process
                processes[0].Kill();
                Console.WriteLine("Explorer process terminated successfully.");
            }


            Stream streamTest = new FileStream(userpath, FileMode.Create, FileAccess.Write, FileShare.None, 0, FileOptions.None);

            StreamWriter streamWriter = new StreamWriter(streamTest, Encoding.UTF8);
            streamWriter.WriteLine(data);
            streamWriter.Flush();
            streamWriter.Close();
            streamTest.Close();

            Console.WriteLine("Data written | Opening file ...");
            //Process.Start("explorer.exe", userpath);

        }


        public async Task WriteMultiple(string[] data, params string[] goodNaming)
        {
            //Console.Clear();
            Console.WriteLine("Writing Multiple Data");

            Console.WriteLine($"DEBUG: names length {goodNaming.Length}");
            Console.WriteLine("What path do you want to save the data?");
            var userpath = Console.ReadLine();

            for (int i = 0; i < data.Length; i++)
            {

                Console.WriteLine($"GOOD NAME {i} {goodNaming[i]}");

                Stream streamTest = new FileStream(userpath + goodNaming[i], FileMode.Create, FileAccess.Write, FileShare.None, 0, FileOptions.None);


                StreamWriter streamWriter = new StreamWriter(streamTest, Encoding.UTF8);
                streamWriter.WriteLine(data[i]);
                streamWriter.Flush();
                streamWriter.Close();
                streamTest.Close();
            }
        }

        public (string,string) FormatRequests(string userURL)
        {
            string[] notAllowed = { "https://", "http://" };
            userURL = userURL.ToLower();
            string defaultURL = GlobalVariables.Url;
            bool changed = false;
            for (int i = 0; i < notAllowed.Length; i++)
            {
                if (userURL.Contains(notAllowed[i]))
                {
                    userURL = userURL.Remove(0, notAllowed[i].Length);
                    Console.WriteLine($"URL fixed: {userURL}");
                    changed = true;
                }
            }

            if (changed == false)
            {
                if (userURL[0] != '/')
                {
                    //userURL = userURL.Substring(1);
                    userURL = "/" + userURL;
                }
                Console.WriteLine($"link wasn't a url, appending {defaultURL} TEST: {userURL[0]}");
                Console.WriteLine($"Appended url looks like: {defaultURL+userURL}");
                return (defaultURL, userURL);
            }
            else
            {
                string[] splitUrl = userURL.Split('/');
                string domain = splitUrl[0];
                string path = "/" + string.Join("/", splitUrl.Skip(1));
                return (domain, path);
            }
        }
    }
}
