using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace HomeBuyingHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var commentRegex = new Regex("<!--.*?-->", RegexOptions.Compiled);
                var wc = new WebClient();
                var filePath = Path.Combine(Environment.CurrentDirectory, "PageContent.txt");
                wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
                var pageContent = wc.DownloadString("https://www.redfin.com/CA/Campbell/1215-Capri-Dr-95008/home/1416725");

                var streetAddressRegex = new Regex(@"<span itemprop=""streetAddress"".*?>(?<Address>.*?)</span>", RegexOptions.Compiled);
                var streetAddress = streetAddressRegex.Match(pageContent).Groups["Address"].Value;

                var cityRegex = new Regex(@"<span itemprop=""addressLocality"".*?>(?<City>.*?)</span>", RegexOptions.Compiled);
                var city = cityRegex.Match(pageContent).Groups["City"].Value;
                city = commentRegex.Replace(city, string.Empty);
                city = city.Replace(",", string.Empty);

                var typeRegx = new Regex(@"<div class=""keyDetail"".*?><span.*?>Property Type</span><span class=""content"".*?>(?<Type>.*?)</span></div>", RegexOptions.Compiled);
                var type = typeRegx.Match(pageContent).Groups["Type"].Value;

                Console.WriteLine($"streetAddress = {streetAddress}");
                Console.WriteLine($"city = {city}");
                Console.WriteLine($"type = {type}");
            }
            catch (WebException we)
            {
                // add some kind of error processing
                Console.WriteLine(we.ToString());
            }
        }
    }
}
