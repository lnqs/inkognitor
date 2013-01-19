using System.Net;
using System.Threading;

namespace Inkognitor
{
    class WebInterface
    {
        private static readonly string Prefix = "http://localhost:8080/";

        private HttpListener httpListener = new HttpListener();
        private Thread worker;
        private bool workerStop;

        public WebInterface()
        {
            httpListener.Prefixes.Add(Prefix);
            worker = new Thread(Run);
        }

        public void Start()
        {
            workerStop = false;
            worker.Start();
        }

        public void Stop()
        {
            workerStop = true;
            worker.Join();
        }

        private void Run()
        {
            httpListener.Start();

            while (!workerStop && httpListener.IsListening)
            {
                HttpListenerContext context = httpListener.GetContext();

                context.Response.StatusCode = 400;
                context.Response.OutputStream.Write(new byte[4] { (byte)'T', (byte)'e', (byte)'s', (byte)'t' }, 0, 4);

                context.Response.OutputStream.Close();
            }

            httpListener.Stop();
        }
    }
}
