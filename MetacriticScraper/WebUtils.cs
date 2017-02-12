using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;

namespace MetacriticScraper
{
    public class WebUtils
    {
        private readonly CookieContainer m_cookieContainer;
        private object m_cookieCheckerLock;

        public WebUtils ()
        {
            m_cookieContainer = new CookieContainer();
            m_cookieCheckerLock = new object();
        }

        private IEnumerable<Cookie> GetCookiesFromContainer()
        {
            var fieldInfo = m_cookieContainer.GetType().GetField("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                Hashtable k = (Hashtable)fieldInfo.GetValue(m_cookieContainer);
                foreach (DictionaryEntry element in k)
                {
                    var field = element.Value.GetType().GetField("m_list", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        foreach (var elem in (SortedList) field.GetValue(element.Value))
                        {
                            var cc = (CookieCollection)((DictionaryEntry)elem).Value;
                            foreach (Cookie c in cc)
                            {
                                yield return c;
                            }
                        }
                    }
                }
            }
        }

        public bool HasCookie (Cookie cookie)
        {
            lock (m_cookieCheckerLock)
            {
                try
                {
                    var cookies = GetCookiesFromContainer();
                    foreach (Cookie c in cookies)
                    {
                        if (c.Name == cookie.Name)
                            return true;
                    }

                    return false;
                }
                catch (Exception e)
                {
                }
                return false;
            }
        }

        public async Task<string> HttpPost(string url, string strPostData, string host, string referer, int timeout)
        {
            string content = "";
            HttpWebRequest objRequest = null;
            var objEncoding = new ASCIIEncoding();
            Stream reqStream = null;
            HttpWebResponse objResponse = null;
            StreamReader objReader = null;

            try
            {
                objRequest = (HttpWebRequest)WebRequest.Create(url);
                objRequest.Method = "POST";
                byte[] objBytes = Encoding.UTF8.GetBytes(new StringBuilder(strPostData).ToString());
                objRequest.CookieContainer = m_cookieContainer;
                objRequest.Timeout = timeout;
                objRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                objRequest.Accept = "application/json, text/javascript, */*; q=0.01";

                if ((objRequest.Headers["Accept-Language"] ?? "").Trim().Length == 0)
                {
                    objRequest.Headers.Add("Accept-Language: en-us,en;q=0.8");
                }

                if ((objRequest.Headers["Accept-Encoding"] ?? "").Trim().Length == 0)
                {
                    objRequest.Headers.Add("Accept-Encoding: gzip, deflate");
                }

                if ((objRequest.Headers["X-Requested-With"] ?? "").Trim().Length == 0)
                {
                    objRequest.Headers.Add("X-Requested-With: XMLHttpRequest");
                }

                if ((objRequest.Headers["Content-Type"] ?? "").Trim().Length == 0)
                {
                    objRequest.Headers.Add("Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
                }

                if (string.IsNullOrEmpty(referer))
                {
                    objRequest.Referer = referer;
                }

                if (string.IsNullOrEmpty(host))
                {
                    objRequest.Host = host;
                }

                objRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

                reqStream = objRequest.GetRequestStream();
                reqStream.Write(objBytes, 0, objBytes.Length);
                reqStream.Close();

                objResponse = (HttpWebResponse) await objRequest.GetResponseAsync();
                objReader = new StreamReader(objResponse.GetResponseStream());
                content = objReader.ReadToEnd();

                foreach (Cookie cook in objResponse.Cookies)
                {
                    if (!HasCookie(cook))
                    {
                        m_cookieContainer.Add(cook);
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                objRequest = null;
                reqStream = null;
                if (objResponse != null)
                    objResponse.Close();
                objResponse = null;
                if (objReader != null)
                    objReader.Close();
                objReader = null;
            }

            return content;
        }

        public async Task<string> HttpGet(string url, string host, string referer, int timeout)
        {
            string content = null;
            try
            {
                var objRequest = (HttpWebRequest)WebRequest.Create(url);
                objRequest.Method = "GET";
                objRequest.CookieContainer = m_cookieContainer;
                objRequest.Timeout = timeout;
                objRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                objRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

                if ((objRequest.Headers["Accept-Language"] ?? "").Trim().Length == 0)
                {
                    objRequest.Headers.Add("Accept-Language: en-us,en;q=0.8");
                }

                if ((objRequest.Headers["Accept-Encoding"] ?? "").Trim().Length == 0)
                {
                    objRequest.Headers.Add("Accept-Encoding: gzip, deflate, sdch");
                }

                if ((objRequest.Headers["Content-Type"] ?? "").Trim().Length == 0)
                {
                    objRequest.Headers.Add("Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
                }

                if (string.IsNullOrEmpty(referer))
                {
                    objRequest.Referer = referer;
                }

                if (string.IsNullOrEmpty(host))
                {
                    objRequest.Host = host;
                }

                objRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

                var result = (HttpWebResponse) await objRequest.GetResponseAsync();

                foreach (Cookie cook in result.Cookies)
                {
                    if (!HasCookie(cook))
                    {
                        m_cookieContainer.Add(cook);
                    }
                }

                Stream resStream = result.GetResponseStream();
                content = GetHtmlContent(resStream);
                result.Close();
                resStream.Close();

                return content;
            }
            catch (Exception e)
            {
            }

            return content;
        }

        protected string GetHtmlContent(Stream htmlStream)
        {
            try
            {
                var sb = new StringBuilder();
                var buf = new byte[32000];
                int count;

                do
                {
                    count = htmlStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        string tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }
                } while (count > 0);

                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
