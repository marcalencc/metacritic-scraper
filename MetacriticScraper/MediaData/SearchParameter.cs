using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class ParameterData : IParameterData
    {
        public string ParameterString { get; set; }

        public ParameterData(string parameterString)
        {
            ParameterString = parameterString;
        }

        public string GetParameterValue(string parameter)
        {
            string[] paramValuePairs = ParameterString.Split('&');
            for (int idx = 0; idx < paramValuePairs.Length; ++idx)
            {
                string[] param = paramValuePairs[idx].Split('=');
                if (param.Length == 2)
                {
                    if (param[0] == parameter)
                    {
                        return param[1];
                    }
                }
            }

            return string.Empty;
        }
    }
}
