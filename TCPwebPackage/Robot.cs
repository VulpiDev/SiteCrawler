using System.Linq;
using System.Text.RegularExpressions;
using TCPwebPackage;

public class Robot
{
    Program program = new Program();
    public async Task Crawl(string website)
    {
        Data dataClass = new Data();
        Console.Clear();
        Console.WriteLine("Begun crawling ...");

        List<string> links = new List<string>();

        Regex linkRegex = new Regex(@"href=\""(.*?)\""");

        foreach (Match match in linkRegex.Matches(website))
        {
            links.Add(match.Groups[1].Value);
        }

        foreach (string link in links)
        {
            Console.WriteLine(link);
        }


        string[] allData = new string[links.Count];

        Console.Clear();
        List<string> formattedLinks = new List<string>();
        for (int i = 0; i < links.Count; i++)
        {
            Console.WriteLine($"{i} : {links[i]}\n");
            (string domain, string page) = dataClass.FormatRequests(links[i]);
            string data = await dataClass.Get(domain,page);
            allData[i] = data;
            formattedLinks.Add(domain + page);
        }

        Console.WriteLine("appropriate naming? Y | N");
        var appropriateNaming = Console.ReadLine();
        appropriateNaming = appropriateNaming.ToLower();
        string[] goodNames = new string[allData.Length];
        if(appropriateNaming == "y")
        {
            goodNames = GetGoodName(formattedLinks);
        }

        goodNames = GetGoodExtension(goodNames);
        await dataClass.WriteMultiple(allData, goodNames);
    }

    public static string[] GetGoodName(List<string> links)
    {
        string[] goodNames = new string[links.Count];
        for (int i = 0; i < links.Count; i++)
        {
            string[] tempLink = links[i].Split('/'); // uonetplus.vulcan.net.pl/powiatrybnicki/
            Console.WriteLine($"Link was: {links[i]}");
            Console.WriteLine($"tempLink: {tempLink[tempLink.Length-1]}");
            Console.WriteLine($"tempLink !0! !(!0!)!: {tempLink[0]}");

            if (tempLink[tempLink.Length - 1] == "")
            {
                Console.WriteLine("This link represents the root of the website");
                goodNames[i] = tempLink[0];
                Console.WriteLine($"Good name {i} is {goodNames[i]} \n");
            }
            else
            {
                goodNames[i] = tempLink[tempLink.Length - 1];
                Console.WriteLine($"Good name {i} is {goodNames[i]} \n");
            }
        }


        return goodNames;
    }
    public static string[] GetGoodExtension(string[] files)
    {

        Console.WriteLine("Would you like to manually assign extensions to unknown files? Y | N");
        var assignExtensions = Console.ReadLine();
        assignExtensions = assignExtensions.ToLower();

        string defaultExtension = ".html";

        string[] websiteExtensions = new string[]
        {
            ".html", ".htm", ".css", ".js",
            ".jpg", ".jpeg", ".png", ".gif", ".svg",
            ".mp4", ".avi", ".mov", ".wmv",
            ".mp3", ".wav", ".ogg",
            ".pdf", ".docx", ".xlsx", ".pptx",
            ".scss", ".less",
            ".ttf", ".otf", ".woff", ".woff2",
            ".json", ".xml", ".csv"
        };

        for (int i = 0; i < files.Length; i++)
        {
            for(int j = 0; j < websiteExtensions.Length; j++)
            {
                if (files[i].Contains(websiteExtensions[j]))
                {
                    break;
                }
                else
                {
                    if (j == websiteExtensions.Length - 1)
                    {
                        if (assignExtensions == "y") //   Assign value if j is at last element
                        {
                            Console.Clear();
                            Console.WriteLine($"It seems like this file doesn't have website extension: {files[i]} \n(ex: .html) Assign value: ");
                            Console.WriteLine("Leave blank if it has extension");
                            var extension = Console.ReadLine();
                            files[i] += $"{extension}";
                            Console.WriteLine($"File {files[i]}");
                        }
                        else
                        {
                            files[i] += $"{defaultExtension}";
                            Console.WriteLine($"File {files[i]}");
                        }
                    }
                }
            }
        }
        files = ReplaceDisallowedCharacters(files);
        return files;
    }

    //  TODO: OPTIMISE THIS:
    public static string[] ReplaceDisallowedCharacters(string[] files)
    {
        char[] invalidChars = { '\\', '/', ':', '*', '?', '\'', '<', '>', '|', '[', ']', '{', '}', '+', '=', ';', ',', '^', '%', '@' };

        string[] fixedFiles = new string[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"WRONG WORD: {files[i]}");
            for (int j = 0; j < invalidChars.Length; j++)
            {
                if (files[i].Contains(invalidChars[j]))
                {
                    files[i] = files[i].Replace(invalidChars[j], '_');
                    Console.WriteLine($"Wrong character deleted: {invalidChars[j]}");
                }
            }
            Console.WriteLine($"corrected string: {files[i]}");
        }

        return files;
    }
}
