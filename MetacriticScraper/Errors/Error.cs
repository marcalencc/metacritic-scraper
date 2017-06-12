using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Errors
{
    class Error
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
