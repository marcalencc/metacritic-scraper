using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Errors
{
    class InvalidUrlException : Exception
    {
        public InvalidUrlException() : base()
        {
        }

        public InvalidUrlException(string message) : base(message)
        {
        }
    }
}
