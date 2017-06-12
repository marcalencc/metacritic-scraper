using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetacriticScraper.RequestData;
using MetacriticScraper.MediaData;
using MetacriticScraper.Interfaces;
using MetacriticScraper.Errors;
using Newtonsoft.Json;

namespace MetacriticScraper
{
    public struct RequestTrackerItem
    {
        string m_requestId;
        public string RequestId
        {
            get
            {
                return m_requestId;
            }
        }
    
        DateTime m_dateAdded;

        public bool IsExpired()
        {
            return (DateTime.Now - m_dateAdded).TotalMilliseconds >= 30000;
        }

        public RequestTrackerItem(string requestId)
        {
            m_requestId = requestId;
            m_dateAdded = DateTime.Now;
        }
    }

    public class RequestQueue<T> : Queue<T>
    {
        private int m_maxRequest;
        private object m_queueLock;
        private AutoResetEvent m_requestSignal;

        public RequestQueue(int maxRequest)
        {
            m_maxRequest = maxRequest;
            m_queueLock = new object();
            m_requestSignal = new AutoResetEvent(false);
        }

        public bool HasAvailableSlot()
        {
            lock (m_queueLock)
            {
                return base.Count < m_maxRequest;
            }
        }

        public new void Enqueue(T item)
        {
            lock (m_queueLock)
            {
                base.Enqueue(item);
            }
            m_requestSignal.Set();
        }

        public new T Dequeue()
        {
            lock (m_queueLock)
            {
                if (base.Count > 0)
                {
                    return base.Dequeue();
                }
            }

            return default(T);
        }

        public bool WaitOnEmpty(int ms)
        {
            return m_requestSignal.WaitOne(ms);
        }
    }

    public class MetacriticScraper
    {
        private string[] MAIN_KEYWORDS = new string[] { "/movie/", "/album/", "/tvshow/", "/person/" };

        private bool m_isRunning;
        private RequestQueue<RequestItem> m_requestQueue;
        private Thread m_requestThread;

        private RequestQueue<IScrapable<MediaItem>> m_dataFetchQueue;
        private Thread m_dataFetchThread;

        private Action<string, string> m_responseChannel;
        private object m_requestTrackerLock;
        private List<RequestTrackerItem> m_requestTracker;
        private System.Threading.Timer m_requestTrackerTimer;

        public MetacriticScraper(Action<string, string> responseChannel)
        {
            m_requestQueue = new RequestQueue<RequestItem>(10);
            m_requestThread = new Thread(RequestThreadProc);

            m_dataFetchQueue = new RequestQueue<IScrapable<MediaItem>>(10);
            m_dataFetchThread = new Thread(DataFetchThreadProc);

            m_requestTracker = new List<RequestTrackerItem>();
            m_requestTrackerLock = new object();
            m_requestTrackerTimer = new Timer(RequestTrackerChecker, null, 0, 30000);
            m_responseChannel = responseChannel;
        }

        private void RequestTrackerChecker(object state)
        {
            lock (m_requestTrackerLock)
            {
                for (int idx = 0; idx < m_requestTracker.Count; ++idx)
                {
                    if(m_requestTracker[idx].IsExpired())
                    {
                        Error error = new Error(new Errors.TimeoutException("Request took too long to be processed"));
                        string resp = JsonConvert.SerializeObject(error);
                        PublishResult(m_requestTracker[idx].RequestId, resp);
                        m_requestTracker.RemoveAt(idx--);
                    }
                }
            }
        }

        public void Initialize()
        {
            m_isRunning = true;
            m_requestThread.Start();
            m_dataFetchThread.Start();
        }

        private RequestItem ParseRequestUrl(string id, string url)
        {
            string keyword = string.Empty;
            for (int idx = 0; idx <= MAIN_KEYWORDS.Length; ++idx)
            {
                if (url.StartsWith(MAIN_KEYWORDS[idx]))
                {
                    keyword = MAIN_KEYWORDS[idx];
                    break;
                }
            }

            string title = string.Empty;
            string yearOrSeason = string.Empty;
            if (!string.IsNullOrEmpty(keyword))
            {
                url = url.Replace(keyword, string.Empty);
                if (!string.IsNullOrEmpty(url))
                {
                    title = url;
                    int slashIdx = url.IndexOf('/');
                    if (slashIdx >= 0)
                    {
                        title = url.Substring(0, slashIdx);
                        url = url.Replace(title + "/", string.Empty);
                        int param;
                        if (!int.TryParse(url, out param))
                        {
                            yearOrSeason = string.Empty;
                        }
                        else
                        {
                            yearOrSeason = param.ToString();
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return CreateRequestItem(id, keyword, title, yearOrSeason.ToString());
        }

        private RequestItem CreateRequestItem(string id, string keyword, string title, string yearOrSeason)
        {
            if (keyword == "/movie/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason) && yearOrSeason.Length == 4)
                {
                   return new MovieRequestItem(id, title, yearOrSeason);
                }
                else
                {
                    return new MovieRequestItem(id, title);
                }
            }
            else if (keyword == "/album/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason) && yearOrSeason.Length == 4)
                {
                    return new AlbumRequestItem(id, title, yearOrSeason);
                }
                else
                {
                    return new AlbumRequestItem(id, title);
                }
            }
            else if (keyword == "/tvshow/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason))
                {
                    return new TVShowRequestItem(id, title, yearOrSeason);
                }
                else
                {
                    return new TVShowRequestItem(id, title);
                }
            }

            return null;
        }

        // Url - no domain name
        public void AddItem(string id, string url)
        {
            if (m_requestQueue.HasAvailableSlot())
            {
                RequestItem req = ParseRequestUrl(id, url);
                if (req != null)
                {
                    m_requestQueue.Enqueue(req);
                    lock (m_requestTrackerLock)
                    {
                        m_requestTracker.Add(new RequestTrackerItem(id));
                    }
                }
                else
                {
                    Error error = new Error(new Errors.InvalidUrlException("Url has invalid format"));
                    string resp = JsonConvert.SerializeObject(error);
                    PublishResult(id, resp);
                }
            }
            else
            {
                // log
            }
        }

        private void RequestThreadProc()
        {
            while (m_isRunning)
            {
                RequestItem item = m_requestQueue.Dequeue();
                if (item != null)
                {
                    ProcessAutoSearch(item);
                }
                else
                {
                    if (!m_requestQueue.WaitOnEmpty(10000))
                    {
                        // log
                    }
                }
                Thread.Sleep(10);
            }
        }

        private void ProcessAutoSearch(RequestItem request)
        {
            var task = request.AutoSearch();
            if (task.Result)
            {
                if (request.FilterValidUrls())
                {
                    m_dataFetchQueue.Enqueue(request);
                }
                else
                {
                    // log
                }
            }
            else
            {
                // log
            }
        }

        private void DataFetchThreadProc()
        {
            while (m_isRunning)
            {
                IScrapable<MediaItem> item = m_dataFetchQueue.Dequeue();
                if (item != null)
                {
                    FetchResults(item);
                }
                else
                {
                    if (!m_dataFetchQueue.WaitOnEmpty(10000))
                    {
                        // log
                    }
                }
                Thread.Sleep(10);
            }
        }

        public async void FetchResults(IScrapable<MediaItem> item)
        {
            List<string> htmlResponses = item.Scrape();
            var tasks = htmlResponses.Select(html => Task.Run(() => item.Parse(html)));

            RequestTrackerItem tItem;
            lock (m_requestTrackerLock)
            {
                tItem = m_requestTracker.FirstOrDefault(i => i.RequestId == item.RequestId);
                if (!EqualityComparer<RequestTrackerItem>.Default.Equals(tItem, default(RequestTrackerItem)))
                {
                    m_requestTracker.Remove(tItem);
                }
            }

            if (!EqualityComparer<RequestTrackerItem>.Default.Equals(tItem, default(RequestTrackerItem)))
            {
                try
                {
                    string resp;
                    MediaItem[] htmlResp = await Task.WhenAll(tasks);
                    if (htmlResp != null && htmlResp.Length > 0)
                    {
                        resp = JsonConvert.SerializeObject(htmlResp);
                    }
                    else
                    {
                        Error error = new Error(new Errors.EmptyResponseException("Empty response"));
                        resp = JsonConvert.SerializeObject(error);
                    }

                    PublishResult(tItem.RequestId, resp);
                }
                catch (Exception)
                {
                    var exceptions = tasks.Where(t => t.Exception != null).Select(t => t.Exception);
                    // log
                }
            }
            else
            {
                // log
            }
        }

        private void PublishResult(string requestId, string result)
        {
            if (m_responseChannel == null)
            {
                // log , exc
            }

            m_responseChannel(requestId, result);
        }
    }
}
