using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Errors
{
    public class TimeoutElapsedException : Exception
    {
        public TimeoutElapsedException() : base()
        {
        }

        public TimeoutElapsedException(string message) : base(message)
        {
        }
    }
}
