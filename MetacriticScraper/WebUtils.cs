using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using MetacriticScraper.Interfaces;
using NLog;

namespace MetacriticScraper.Web
{
    public class WebUtils : IWebUtils
    {
        private static HttpClient m_httpClient;
        private static HttpClientHandler m_handler;
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public WebUtils()
        {
            m_handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            m_httpClient = new HttpClient(m_handler, true);
            m_httpClient.DefaultRequestHeaders.ConnectionClose = false;
            m_httpClient.Timeout = new TimeSpan(0, 0, 0, 0, 30000);
            m_httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-us,en;q=0.8");
            m_httpClient.DefaultRequestHeaders.Host = Constants.MetacriticDomain;

            Logger.Info("WebUtils Initialized...");
        }

        private async Task<string> SendAsync(HttpRequestMessage request)
        {
            try
            {
                HttpResponseMessage response = await m_httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Logger.Error("HttpRequestException encountered wgile sending request {0}. Error: {1}",
                    request.RequestUri, ex.ToString());
            }

            return null;
        }

        public async Task<string> HttpPost(string url, string strPostData, string referer, int timeout)
        {
            Logger.Debug("Posting to {0} -> PostdData: {1}", url, strPostData);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            byte[] postData = Encoding.UTF8.GetBytes(strPostData);
            ByteArrayContent content = new ByteArrayContent(postData);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentLength = postData.Length;
            request.Content = content;

            request.Headers.Add("Accept-Language", "en-us,en;q=0.8");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/javascript"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.01));
            request.Headers.Referrer = new Uri(referer);

            return await SendAsync(request);
        }

        public async Task<string> HttpGet(string url, string referer, int timeout)
        {
            Logger.Debug("Getting {0} ", url);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            request.Headers.Referrer = new Uri(referer);

            return await SendAsync(request);
        }
    }
}
