using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper
{
    public class Result
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string ItemDate { get; set; }
        public string ImagePath { get; set; }
        public int? MetaScore { get; set; }
        public string ScoreWord { get; set; }
        public string RefType { get; set; }
        public int RefTypeId { get; set; }
    }
}
