using System;
using System.Net;

namespace Inkognitor
{
    class WebInterface
    {
        private static readonly string Prefix = "http://localhost:8080/";

        private HttpListener httpListener = new HttpListener();

        public WebInterface()
        {
            httpListener.Prefixes.Add(Prefix);
        }

        public void Start()
        {
            httpListener.Start();
            httpListener.BeginGetContext(HandleNewRequest, null);
        }

        public void Stop()
        {
            httpListener.Stop();
        }

        private void HandleNewRequest(IAsyncResult ar)
        {
            HttpListenerContext context = httpListener.EndGetContext(ar);
            context.Response.StatusCode = 400;
            context.Response.OutputStream.Write(new byte[4] { (byte)'T', (byte)'e', (byte)'s', (byte)'t' }, 0, 4);
            context.Response.OutputStream.Close();
        }
    }
}
