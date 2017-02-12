using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetacriticScraper.RequestData;

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
                return base.Dequeue();
            }
        }
    }


    public class MetacriticScraper
    {
        private RequestQueue<IMediaItemRequest> m_requestQueue;
        private Thread m_requestThread;
        private AutoResetEvent m_requestSignal;
        private bool m_isRunning;

        public MetacriticScraper()
        {
            m_requestQueue = new RequestQueue<IMediaItemRequest>(10);
            m_requestThread = new Thread(RequestThreadProc);
            m_requestSignal = new AutoResetEvent(false);
        }

        public void Initialize()
        {
            m_isRunning = true;
            m_requestThread.Start();
        }

        public void AddItem(string url)
        {
            if (m_requestQueue.HasAvailableSlot())
            {
                m_requestQueue.Enqueue(new MovieRequest(url));
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
                IMediaItemRequest item = m_requestQueue.Dequeue();
                if (item != null)
                {
                    ProcessRequest(item);
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

        private void ProcessRequest(IMediaItemRequest request)
        {

        }

    }
}
