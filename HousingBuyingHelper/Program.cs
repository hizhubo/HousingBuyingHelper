using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper;

namespace HousingBuyingHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var webScraper = new SinglePageScraper();

            var paymentPerMonth = new Selector();
            paymentPerMonth.Name = "Payment Per Month";
            paymentPerMonth.Type = SelectorType.CssSelector;
            paymentPerMonth.Expression = "div.major-part div.title";
            paymentPerMonth.Regex = @"\$(?<Value>.*?) per month";

            var paymentCalculatorSelector = new Selector();
            paymentCalculatorSelector.Type = SelectorType.ClassName;
            paymentCalculatorSelector.Expression = "MortgageCalculator";
            paymentCalculatorSelector.SubSelectors = new[] { paymentPerMonth };

            var streetAddressSelector = new Selector();
            streetAddressSelector.Name = "Street Address";
            streetAddressSelector.Type = SelectorType.ClassName;
            streetAddressSelector.Expression = "street-address";

            var selectors = new[] { streetAddressSelector, paymentCalculatorSelector };

            webScraper.Scrape("https://www.redfin.com/CA/Campbell/1215-Capri-Dr-95008/home/1416725", selectors);

            Console.WriteLine($"{streetAddressSelector.Name} = {streetAddressSelector.Value}");
            Console.WriteLine($"{paymentPerMonth.Name} = {paymentPerMonth.Value}");
        }
    }
}
