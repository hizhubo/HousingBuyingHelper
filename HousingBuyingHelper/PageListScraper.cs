﻿using HousingBuyingHelper.RedfinApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using WebScraper;

namespace HousingBuyingHelper
{
    public class PageListScraper
    {
        public List<List<Selector>> Scrape(string url)
        {
            string apiUrl = this.GetApiUrl(url);
            var selectorRows = this.GetSelectorRows(apiUrl);

            return selectorRows;
        }

        private string GetApiUrl(string url)
        {
            var apiUrlRegex = new Regex(@"""(?<ApiUrl>\\u002Fstingray\\u002Fapi\\u002Fgis.*?)""", RegexOptions.Compiled);
            var pageContent = this.GetPageContent(url);
            var apiUrl = apiUrlRegex.Match(pageContent).Groups["ApiUrl"].Value;
            apiUrl = $"https://www.redfin.com{apiUrl.Replace("\\u002F", "/")}";

            return apiUrl;
        }

        private List<List<Selector>> GetSelectorRows(string apiUrl)
        {
            var apiResponseContent = this.GetPageContent(apiUrl);
            apiResponseContent = PreProcessApiResponseContent(apiResponseContent);
            var apiResponse = JsonConvert.DeserializeObject<RedfinApiResponse>(apiResponseContent);

            var selectorRows = new List<List<Selector>>();

            foreach (var home in apiResponse.payload.homes)
            {
                var selectorRow = new List<Selector>();
                selectorRow.Add(new Selector { Name = "Url", Value = $"https://www.redfin.com{home.url}" });
                selectorRow.Add(new Selector { Name = "Latitude", Value = home.latLong.value.latitude });
                selectorRow.Add(new Selector { Name = "Longitude", Value = home.latLong.value.longitude });
                selectorRows.Add(selectorRow);
            }

            return selectorRows;
        }

        private string GetPageContent(string url)
        {
            var wc = new WebClient();
            wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            var pageContent = wc.DownloadString(url);

            return pageContent;
        }

        private static string PreProcessApiResponseContent(string apiResponseContent)
        {
            apiResponseContent = apiResponseContent.Replace("{}&&", string.Empty);
            apiResponseContent = apiResponseContent.Replace("鈥?,", "\",");
            apiResponseContent = apiResponseContent.Replace("鈥", string.Empty);
            return apiResponseContent;
        }
    }
}
