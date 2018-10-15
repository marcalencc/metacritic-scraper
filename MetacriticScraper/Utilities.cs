using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper
{
    public static class Constants
    {
        public static string MetacriticURL = @"https://www.metacritic.com";
        public static string MetacriticDomain = @"www.metacritic.com";
        public static int MovieTypeId = 150;
        public static int TvShowTypeId = 70;
        public static int AlbumTypeId = 170;
        public static int PersonTypeId = 110;
        public static int MAX_LIMIT = 20;
        public static int DEFAULT_LIMIT = 20;
    }

    public interface IResult : IEquatable<IResult>
    {
        string Name { get; }
        int RefTypeId { get; }
        string ItemDate { get; }
    }
}
