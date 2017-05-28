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
            Exception = exception;
            Message = exception.Message;
        }

        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
