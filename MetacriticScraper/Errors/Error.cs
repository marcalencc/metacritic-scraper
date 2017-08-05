using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.Errors
{
    public class Error: IMetacriticData
    {
        public Error (Exception exception)
        {
            Exception = exception.ToString();
            Message = exception.Message;
        }

        public string Exception { get; set; }
        public string Message { get; set; }
    }
}
