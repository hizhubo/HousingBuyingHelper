using HousingBuyingHelper.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebScraper;

namespace HousingBuyingHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            // ScrapeTest();

            ScrapeAll();
        }

        private static void ScrapeTest()
        {
            var webScraperContent = File.ReadAllText("WebScraperConfig.xml");
            var webScraper = new SinglePageScraper();
            var singlePageUrl = "https://www.redfin.com/CA/San-Jose/127-Herlong-Ave-95123/home/1287770";
            var selectors = new List<Selector> { new Selector { Name = "Url", Value = singlePageUrl } };
            var webScraperConfig = GetSelectors(webScraperContent, selectors);
            webScraper.Scrape(singlePageUrl, webScraperConfig);

            var rowValues = new List<string>();
            GenerateCsvRow(webScraperConfig, rowValues);
            Console.WriteLine(string.Join(",", rowValues));
        }

        private static void ScrapeAll()
        {
            var homesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Homes_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.csv");
            var webScraperContent = File.ReadAllText("WebScraperConfig.xml");
            OutputCsvHeader(homesFilePath, webScraperContent);
            var singlePageSelectorGroups = GetSelectorGroups();
            var webScraper = new SinglePageScraper();

            foreach (var singlePageSelectors in singlePageSelectorGroups)
            {
                var url = singlePageSelectors.First(s => s.Name == "Url").Value;
                var webScraperConfig = GetSelectors(webScraperContent, singlePageSelectors);
                webScraper.Scrape(url, webScraperConfig);
                GetRoute(webScraperConfig);
                Output(webScraperConfig, homesFilePath);
            }
        }

        private static void GetRoute(List<Selector> webScraperConfig)
        {
            try
            {
                var routeCalculator = new RouteCalculator();
                var latitudeSelector = webScraperConfig.First(s => s.Name == "Latitude");
                var LongitudeSelector = webScraperConfig.First(s => s.Name == "Longitude");

                var distanceToKeysight = webScraperConfig.First(s => s.Name == "Distance to Keysight (mi)");
                var distanceToIntel = webScraperConfig.First(s => s.Name == "Distance to Intel (mi)");
                var durationToKeysight = webScraperConfig.First(s => s.Name == "Duration to Keysight (min)");
                var durationToIntel = webScraperConfig.First(s => s.Name == "Duration to Intel (min)");
                var origin = $"{latitudeSelector.Value},{LongitudeSelector.Value}";

                var routeToKeysight = routeCalculator.GetRoute(origin, "5301 Stevens Creek Blvd, Santa Clara");

                if (routeToKeysight != null)
                {
                    distanceToKeysight.Value = routeToKeysight.DistanceInMiles.ToString("N1");
                    durationToKeysight.Value = routeToKeysight.DurationInMinutes.ToString("N1");
                }

                var routeToIntel = routeCalculator.GetRoute(origin, "3065 Bowers Ave, Santa Clara");

                if (routeToIntel != null)
                {
                    distanceToIntel.Value = routeToIntel.DistanceInMiles.ToString("N1");
                    durationToIntel.Value = routeToIntel.DurationInMinutes.ToString("N1");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void OutputCsvHeader(string homesFilePath, string webScraperContent)
        {
            var webScraperConfig = GetSelectors(webScraperContent, null);
            var names = new List<string>();
            GenerateCsvHeader(webScraperConfig, names);
            File.WriteAllText(homesFilePath, string.Join(",", names));
        }

        private static List<Selector> GetSelectors(string webScraperContent, List<Selector> selectors)
        {
            var webScraperConfig = webScraperContent.Deserialize<Selector[]>().ToList();
            webScraperConfig.Add(selectors == null ? new Selector { Name = "Latitude" } : selectors.FirstOrDefault(s => s.Name == "Latitude"));
            webScraperConfig.Add(selectors == null ? new Selector { Name = "Longitude" } : selectors.First(s => s.Name == "Longitude"));
            webScraperConfig.Add(new Selector { Name = "Distance to Keysight (mi)" });
            webScraperConfig.Add(new Selector { Name = "Distance to Intel (mi)" });
            webScraperConfig.Add(new Selector { Name = "Duration to Keysight (min)" });
            webScraperConfig.Add(new Selector { Name = "Duration to Intel (min)" });
            webScraperConfig.Add(selectors == null ? new Selector { Name = "Url" } : selectors.First(s => s.Name == "Url"));

            return webScraperConfig;
        }

        private static List<List<Selector>> GetSelectorGroups()
        {
            var selectorGroups = new List<List<Selector>>();
            var pageListScraper = new PageListScraper();
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/19457/CA/Sunnyvale"));
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/4561/CA/Cupertino"));
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/12739/CA/Mountain-View"));
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/11234/CA/Los-Gatos"));
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/17675/CA/Santa-Clara"));
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/17420/CA/San-Jose"));
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/2673/CA/Campbell"));
            selectorGroups.AddRange(pageListScraper.Scrape("https://www.redfin.com/city/12204/CA/Milpitas"));

            return selectorGroups;
        }

        static void GenerateCsvHeader(List<Selector> selectors, List<string> names)
        {
            foreach (var selector in selectors)
            {
                if (!string.IsNullOrWhiteSpace(selector.Name))
                {
                    names.Add(selector.Name);
                }

                if (selector.SubSelectors != null && selector.SubSelectors.Count > 0)
                {
                    GenerateCsvHeader(selector.SubSelectors, names);
                }
            }
        }

        static void Output(List<Selector> selectors, string filePath)
        {
            var rowValues = new List<string>();
            GenerateCsvRow(selectors, rowValues);
            var row = string.Join(",", rowValues);
            Console.WriteLine(row);
            File.AppendAllText(filePath, $"{Environment.NewLine}{row}");
        }

        static void GenerateCsvRow(List<Selector> selectors, List<string> values)
        {
            foreach (var selector in selectors)
            {
                if (!string.IsNullOrWhiteSpace(selector.Name))
                {
                    if (selector.Value == "—")
                    {
                        selector.Value = "N/A";
                    }

                    values.Add($"\"{selector.Value}\"");
                }

                if (selector.SubSelectors != null && selector.SubSelectors.Count > 0)
                {
                    GenerateCsvRow(selector.SubSelectors, values);
                }
            }
        }
    }
}
