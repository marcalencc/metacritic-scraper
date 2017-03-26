using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper
{
    public class Result : IResult
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string ItemDate { get; set; }
        public string ImagePath { get; set; }
        public int? MetaScore { get; set; }
        public string ScoreWord { get; set; }
        public string RefType { get; set; }
        public int RefTypeId { get; set; }

        public bool Equals(IResult obj)
        {
            bool result = false;
            if (obj == null)
            {
                return false;
            }
            else if (RefTypeId != obj.RefTypeId)
            {
                return false;
            }
            else if (RefTypeId == Constants.MovieTypeId ||
                RefTypeId == Constants.TvShowTypeId)
            {
                result = string.Equals(Name, obj.Name); 
            }
            else if (RefTypeId == Constants.AlbumTypeId)
            {
                string name = obj.Name.Split('-')[0].Trim();
                result = string.Equals(Name, obj.Name);
            }

            if (result && obj.ItemDate != null)
            {
                result = string.Equals(ItemDate, obj.ItemDate);
            }

            return result;
        }
    }
}
