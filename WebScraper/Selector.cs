using System.Collections.Generic;

namespace WebScraper
{
    public class Selector
    {
        public List<Selector> SubSelectors { get; set; }

        public string Name { get; set; }

        public string Expression { get; set; }

        public string Value { get; set; }

        public SelectorType Type { get; set; }

        public string Regex { get; set; }
    }
}
