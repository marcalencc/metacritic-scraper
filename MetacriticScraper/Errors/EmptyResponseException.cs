using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Errors
{
    public class EmptyResponseException : Exception
    {
        public EmptyResponseException() : base()
        {
        }

        public EmptyResponseException(string message) : base(message)
        {
        }
    }
}
