using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.MediaData;

namespace MetacriticScraper.Interfaces
{
    interface IMediaItem
    {
        string Title { get; set; }
        List<Credit> Credits { get; set; }
        DateTime ReleaseDate { get; set; }
        Rating Rating { get; set; }
    }
}
