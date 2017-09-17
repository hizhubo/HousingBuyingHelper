using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System.Text.RegularExpressions;

namespace WebScraper
{
    public class SinglePageScraper
    {
        public void Scrape(string url, Selector[] selectors)
        {
            using (var webDriver = new InternetExplorerDriver())
            {
                webDriver.Url = url;
                webDriver.Navigate();
                IWebElement webElement = webDriver.FindElementByTagName("html");

                foreach (var selector in selectors)
                {
                    this.ScrapeWebElement(webElement, selector);
                }

                webDriver.Quit();
            }
        }

        private void ScrapeWebElement(IWebElement webElement, Selector selector)
        {
            try
            {
                webElement = this.FindWebElement(webElement, selector);
            }
            catch (NoSuchElementException)
            {
                selector.Value = "N/A";

                return;
            }

            if (!string.IsNullOrWhiteSpace(selector.Name))
            {
                selector.Value = webElement.Text;
            }

            if (!string.IsNullOrWhiteSpace(selector.Regex))
            {
                selector.Value = new Regex(selector.Regex).Match(selector.Value).Groups["Value"].Value;
            }

            if (selector.SubSelectors != null && selector.SubSelectors.Length > 0)
            {
                foreach (var subSelector in selector.SubSelectors)
                {
                    this.ScrapeWebElement(webElement, subSelector);
                }
            }
        }

        private IWebElement FindWebElement(IWebElement webElement, Selector selector)
        {
            switch (selector.Type)
            {
                case SelectorType.ClassName:
                    webElement = webElement.FindElement(By.ClassName(selector.Expression));
                    break;
                case SelectorType.CssSelector:
                    webElement = webElement.FindElement(By.CssSelector(selector.Expression));
                    break;
                case SelectorType.Id:
                    webElement = webElement.FindElement(By.Id(selector.Expression));
                    break;
                case SelectorType.LinkText:
                    webElement = webElement.FindElement(By.LinkText(selector.Expression));
                    break;
                case SelectorType.Name:
                    webElement = webElement.FindElement(By.Name(selector.Expression));
                    break;
                case SelectorType.PartialLinkText:
                    webElement = webElement.FindElement(By.PartialLinkText(selector.Expression));
                    break;
                case SelectorType.TagName:
                    webElement = webElement.FindElement(By.TagName(selector.Expression));
                    break;
                case SelectorType.XPath:
                    webElement = webElement.FindElement(By.XPath(selector.Expression));
                    break;
            }

            return webElement;
        }
    }
}
