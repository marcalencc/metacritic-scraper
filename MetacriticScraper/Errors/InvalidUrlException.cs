using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Errors
{
    public class InvalidUrlException : Exception
    {
        public InvalidUrlException() : base()
        {
        }

        public InvalidUrlException(string message) : base(message)
        {
        }

        public InvalidUrlException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
