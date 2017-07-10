using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Errors
{
    class SystemBusyException : Exception
    {
        public SystemBusyException() : base()
        {
        }

        public SystemBusyException(string message) : base(message)
        {
        }
    }
}
