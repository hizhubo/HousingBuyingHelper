namespace WebScraper
{
    public class Selector
    {
        public Selector[] SubSelectors { get; set; }

        public string Name { get; set; }

        public string Expression { get; set; }

        public string Value { get; set; }

        public SelectorType Type { get; set; }

        public string Regex { get; set; }
    }
}
