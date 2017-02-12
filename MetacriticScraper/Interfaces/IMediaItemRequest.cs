using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.RequestData
{
    public interface IMediaItemRequest
    {
        string SearchString { get; }
        string ReleaseYear { get; }
    }
}
