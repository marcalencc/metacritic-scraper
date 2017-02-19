using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.JSONObjects
{
    public class RootObject
    {
        public AutoComplete AutoComplete { get; set; }
    }

    public class AutoComplete
    {
        public Totals Totals { get; set; }
        public List<Result> Results { get; set; }
    }
}
