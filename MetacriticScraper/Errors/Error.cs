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
            m_exception = exception.ToString();
            Message = exception.Message;
        }

        private string m_exception;
        public string Exception
        {
            get
            {
                return m_exception.Substring(0, m_exception.IndexOf(':'));
            }
            set
            {
                m_exception = value;
            }
        }

        public string Message { get;}
    }
}
