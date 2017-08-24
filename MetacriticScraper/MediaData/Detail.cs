using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class MediaDetail : IMetacriticData
    {
        private List<DetailItem> m_details;
        public List<DetailItem> Details
        {
            get
            {
                return m_details;
            }
        }

        public bool ShouldSerializeDetails()
        {
            return m_details.Count > 0;
        }

        private List<MediaCredit> m_credits;
        public List<MediaCredit> Credits
        {
            get
            {
                return m_credits;
            }
        }

        public bool ShouldSerializeCredits()
        {
            return m_credits.Count > 0;
        }

        public MediaDetail()
        {
            m_details = new List<DetailItem>();
            m_credits = new List<MediaCredit>();
        }
    }

    public class MediaCredit : IEquatable<MediaCredit>
    {
        public MediaCredit(string name, string credit)
        {
            m_name = name.Trim();
            m_credit = credit.Trim();
        }

        private string m_name;
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        private string m_credit;
        public string Credit
        {
            get
            {
                return m_credit;
            }
        }

        public bool Equals(MediaCredit other)
        {
            return m_name == other.m_name &&
                m_credit == other.m_credit;
        }
    }

    public class DetailItem : IEquatable<DetailItem>
    {
        public DetailItem(string description, string value)
        {
            m_description = description.Trim();
            m_value = value.Trim();
        }

        private string m_description;
        public string Description
        {
            get
            {
                return m_description;
            }
        }

        private string m_value;
        public string Value
        {
            get
            {
                return m_value;
            }
        }

        public bool Equals(DetailItem other)
        {
            return m_description == other.m_description &&
                m_value == other.m_value;
        }
    }
}