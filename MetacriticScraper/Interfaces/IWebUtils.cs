using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Interfaces
{
    public interface IWebUtils
    {
        Task<string> HttpPost(string url, string strPostData, string referer, int timeout);

        Task<string> HttpGet(string url, string referer, int timeout);
    }
}
