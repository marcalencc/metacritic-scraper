using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetacriticScraper.MediaData
{
    public class Detail : IEquatable<Detail>
    {
        public Detail(string description, string value)
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

        public bool Equals(Detail other)
        {
            return m_description == other.m_description &&
                m_value == other.m_value;
        }
    }
}