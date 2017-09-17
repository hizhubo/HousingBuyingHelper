using HousingBuyingHelper.Extension;
using System;
using System.IO;
using WebScraper;

namespace HousingBuyingHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var webScraper = new SinglePageScraper();
            var webScraperContent = File.ReadAllText("WebScraperConfig.xml");
            var webScraperConfig = webScraperContent.Deserialize<Selector[]>();
            
            /*
             * Apartment
            webScraper.Scrape("https://www.redfin.com/CA/Campbell/1215-Capri-Dr-95008/home/1416725", webScraperConfig);
            */

            /*
             Single Family House
             */
            webScraper.Scrape("https://www.redfin.com/CA/Sunnyvale/975-Blazingwood-Dr-94089/home/1510191", webScraperConfig);

            /*
             Single Family House_more than 3 schools
            webScraper.Scrape("https://www.redfin.com/CA/Los-Gatos/Santa-Ana-Rd-95033/home/22462685", webScraperConfig);
             */

            Output(webScraperConfig);
        }

        static void Output(Selector[] selectors)
        {
            foreach (var selector in selectors)
            {
                if (!string.IsNullOrWhiteSpace(selector.Name))
                {
                    Console.WriteLine($"{selector.Name} = {selector.Value}");
                }

                if (selector.SubSelectors != null && selector.SubSelectors.Length > 0)
                {
                    Output(selector.SubSelectors);
                }
            }
        }
    }
}
