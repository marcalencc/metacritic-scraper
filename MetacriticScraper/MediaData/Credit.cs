using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetacriticScraper.MediaData
{
    public class Credit
    {
        private string m_credit;
        public string ActualCredit
        {
            get
            {
                return m_credit;
            }
        }

        private string m_person;
        public string Person
        {
            get
            {
                return m_person;
            }
        }
    }
}