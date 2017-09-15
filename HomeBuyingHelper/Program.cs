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

                var type = GetPropertyValue(pageContent, "Property Type");
                var mslNo = GetPropertyValue(pageContent, "MLS#");
                var lotSize = GetPropertyValue(pageContent, "Lot Size");
                var builtinYear = GetPropertyValue(pageContent, "Built");
                var paymentMonthly = GetPaymentValue(pageContent, "Monthly");
                var paymentPrincipleAndInterest = GetPaymentValue(pageContent, "Principal and Interest");
                var paymentPropertyTaxes = GetPaymentValue(pageContent, "Property Taxes");
                var paymentHoaDues = GetPaymentValue(pageContent, "HOA Dues");
                var paymentHomeownersInsurance = GetPaymentValue(pageContent, "Homeowners' Insurance");

                Console.WriteLine($"streetAddress = {streetAddress}");
                Console.WriteLine($"city = {city}");
                Console.WriteLine($"type = {type}");
                Console.WriteLine($"mslNo = {mslNo}");
                Console.WriteLine($"lotSize = {lotSize}");
                Console.WriteLine($"builtinYear = {builtinYear}");
                Console.WriteLine($"paymentMonthly = {paymentMonthly}");
                Console.WriteLine($"paymentPrincipleAndInterest = {paymentPrincipleAndInterest}");
                Console.WriteLine($"paymentPropertyTaxes = {paymentPropertyTaxes}");
                Console.WriteLine($"paymentHoaDues = {paymentHoaDues}");
                Console.WriteLine($"paymentHomeownersInsurance = {paymentHomeownersInsurance}");
            }
            catch (WebException we)
            {
                // add some kind of error processing
                Console.WriteLine(we.ToString());
            }
        }

        static string GetPropertyValue(string pageContent, string name)
        {
            var cellRegx = new Regex($@"<div class=""keyDetail"".*?><span.*?>{name}</span><span class=""content"".*?>(?<Value>.*?)</span></div>", RegexOptions.Compiled);
            var value = cellRegx.Match(pageContent).Groups["Value"].Value;

            return value;
        }

        static string GetPaymentValue(string pageContent, string name)
        {
            var paymentSectionRegex = new Regex(@"<div class=""MortgageCalculatorSummary"".*?>.*?<div class=""MortgageCalculatorForm""", RegexOptions.Compiled | RegexOptions.Multiline);
            var paymentSectionContent = paymentSectionRegex.Match(pageContent).Value;

            if (name == "Monthly")
            {
                var monthlyPaymentRegx = new Regex(@"<div class=""title"">\$(?<Price>.*?) per month</div>", RegexOptions.Compiled);

                return monthlyPaymentRegx.Match(paymentSectionContent).Groups["Price"].Value;
            }

            var cellRegx = new Regex($@"<div class=""dot-value-row"">.*?{name}.*?<span class=""value"">\$(?<Value>.*?)</span>", RegexOptions.Compiled);
            var value = cellRegx.Match(paymentSectionContent).Groups["Value"].Value;

            return value;
        }
    }
}
