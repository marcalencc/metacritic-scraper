using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetacriticScraper.RequestData;
using MetacriticScraper.MediaData;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper
{
    public class RequestQueue<T> : Queue<T>
    {
        private int m_maxRequest;
        private object m_queueLock;

        public RequestQueue(int maxRequest)
        {
            m_maxRequest = maxRequest;
            m_queueLock = new object();
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
    }

    public class MetacriticScraper
    {
        private string[] MAIN_KEYWORDS = new string[] { "/movie/", "/album/", "/tvshow/", "/person/" };

        private bool m_isRunning;
        private RequestQueue<RequestItem> m_requestQueue;
        private Thread m_requestThread;
        private AutoResetEvent m_requestSignal;

        private RequestQueue<IScrapable<MediaItem>> m_dataFetchQueue;
        private Thread m_dataFetchThread;
        private AutoResetEvent m_dataFetchSignal;

        public MetacriticScraper()
        {
            m_requestQueue = new RequestQueue<RequestItem>(10);
            m_requestThread = new Thread(RequestThreadProc);
            m_requestSignal = new AutoResetEvent(false);

            m_dataFetchQueue = new RequestQueue<IScrapable<MediaItem>>(10);
            m_dataFetchThread = new Thread(DataFetchThreadProc);
            m_dataFetchSignal = new AutoResetEvent(false);
        }

        public void Initialize()
        {
            m_isRunning = true;
            m_requestThread.Start();
            m_dataFetchThread.Start();
        }

        private RequestItem ParseRequestUrl(string url)
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

            if (!string.IsNullOrEmpty(keyword))
            {
                string title = string.Empty;
                url = url.Replace(keyword, string.Empty);

                title = url;
                int slashIdx = url.IndexOf('/');
                if (slashIdx >= 0)
                {
                    title = url.Substring(0, slashIdx - 1);
                    url = url.Replace(title, string.Empty);
                    int year;
                    int.TryParse(url, out year);
                }
            }

            return null;
        }

        // Url - no domain name
        public void AddItem(string url)
        {
            if (m_requestQueue.HasAvailableSlot())
            {

                m_requestQueue.Enqueue(new TVShowRequestItem(url));
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
                    if (!m_requestSignal.WaitOne(10000))
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
                    if (!m_requestSignal.WaitOne(10000))
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

            try
            {
                MediaItem[] htmlResp = await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                var exceptions = tasks.Where(t => t.Exception != null).Select(t => t.Exception);
                // log
            }
        }
    }
}
