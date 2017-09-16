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

            var streetAddressSelector = new Selector();
            streetAddressSelector.Name = "Street Address";
            streetAddressSelector.Type = SelectorType.ClassName;
            streetAddressSelector.Expression = "street-address";

            var citySelector = new Selector();
            citySelector.Name = "City";
            citySelector.Type = SelectorType.CssSelector;
            citySelector.Expression = "span[itemprop='addressLocality']";
            citySelector.Regex = "(?<Value>.*?),";

            var statusSelector = new Selector();
            statusSelector.Name = "Status";
            statusSelector.Type = SelectorType.CssSelector;
            statusSelector.Expression = "span.status-container span.value span[data-reactid='68']";

            var priceListedSelector = new Selector();
            priceListedSelector.Name = "Listed Price";
            priceListedSelector.Type = SelectorType.CssSelector;
            priceListedSelector.Expression = "span[itemprop='price']";

            var priceEstimatedSelector = new Selector();
            priceEstimatedSelector.Name = "Redfin Estimated Price";
            priceEstimatedSelector.Type = SelectorType.CssSelector;
            priceEstimatedSelector.Expression = "span[data-rf-test-id='avmLdpPrice'] span.value";
            priceEstimatedSelector.Regex = @"\$(?<Value>.*)";

            var paymentPerMonth = new Selector();
            paymentPerMonth.Name = "Payment Per Month";
            paymentPerMonth.Type = SelectorType.CssSelector;
            paymentPerMonth.Expression = "div.major-part div.title";
            paymentPerMonth.Regex = @"\$(?<Value>.*?) per month";

            var paymentPrincipalAndInterest = new Selector();
            paymentPrincipalAndInterest.Name = "Principal and Interest";
            paymentPrincipalAndInterest.Type = SelectorType.CssSelector;
            paymentPrincipalAndInterest.Expression = "div.dot-value-container div.dot-value-row:nth-of-type(1) span.value";
            paymentPrincipalAndInterest.Regex = @"\$(?<Value>.*)";

            var paymentPropertyTaxes = new Selector();
            paymentPropertyTaxes.Name = "Property Taxes";
            paymentPropertyTaxes.Type = SelectorType.CssSelector;
            paymentPropertyTaxes.Expression = "div.dot-value-container div.dot-value-row:nth-of-type(2) span.value";
            paymentPropertyTaxes.Regex = @"\$(?<Value>.*)";

            var paymentHoaDues = new Selector();
            paymentHoaDues.Name = "HOA Dues";
            paymentHoaDues.Type = SelectorType.CssSelector;
            paymentHoaDues.Expression = "div.dot-value-container div.dot-value-row:nth-of-type(3) span.value";
            paymentHoaDues.Regex = @"\$(?<Value>.*)";

            var paymentHomeownersInsurance = new Selector();
            paymentHomeownersInsurance.Name = "Homeowners' Insurance";
            paymentHomeownersInsurance.Type = SelectorType.CssSelector;
            paymentHomeownersInsurance.Expression = "div.dot-value-container div.dot-value-row:nth-of-type(4) span.value";
            paymentHomeownersInsurance.Regex = @"\$(?<Value>.*)";

            var paymentCalculatorSelector = new Selector();
            paymentCalculatorSelector.Type = SelectorType.ClassName;
            paymentCalculatorSelector.Expression = "MortgageCalculator";
            paymentCalculatorSelector.SubSelectors = new[] { paymentPerMonth, paymentPrincipalAndInterest, paymentPropertyTaxes, paymentHoaDues, paymentHomeownersInsurance };

            var selectors = new[] { streetAddressSelector, citySelector, statusSelector, priceListedSelector, priceEstimatedSelector, paymentCalculatorSelector, paymentPropertyTaxes };
            webScraper.Scrape("https://www.redfin.com/CA/Campbell/1215-Capri-Dr-95008/home/1416725", selectors);

            Output(selectors);
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
