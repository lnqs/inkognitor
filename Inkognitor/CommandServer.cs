using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Linq;

namespace Inkognitor
{
    public class CommandServer : ICommandProvider
    {
        static private readonly int BufferSize = 1024;

        private TcpListener tcpListener;
        private List<Connection> connections = new List<Connection>();

        public CommandServer(IPAddress localaddr, int port)
        {
            tcpListener = new TcpListener(localaddr, port);
        }

        public CommandProviderCallback Dispatch { get; set; }

        public void Start()
        {
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(HandleAccept, tcpListener);
        }

        public void Stop()
        {
            lock (tcpListener)
            {
                tcpListener.Stop();
            }

            lock (connections)
            {
                foreach (Connection connection in connections)
                {
                    lock (connection)
                    {
                        connection.Client.Close();
                    }
                }

                connections.Clear();
            }
        }

        private void HandleAccept(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client;

            lock (listener)
            {
                client = listener.EndAcceptTcpClient(ar);
                tcpListener.BeginAcceptTcpClient(HandleAccept, listener);
            }

            Connection connection = new Connection(client);
            connection.Stream.BeginRead(connection.RecvBuffer, 0,
                    connection.RecvBuffer.Length, HandleRead, connection);

            lock (connections)
            {
                connections.Add(connection);
            }
        }

        private void HandleRead(IAsyncResult ar)
        {
            Connection connection = (Connection)ar.AsyncState;

            lock (connection)
            {
                int read = connection.Stream.EndRead(ar);

                if (read > 0)
                {
                    connection.RecvBufferCount += read;
                    while (findAndHandleCommands(connection)) {}
                    connection.Stream.BeginRead(connection.RecvBuffer, connection.RecvBufferCount,
                            connection.RecvBuffer.Length - connection.RecvBufferCount, HandleRead, connection);
                }
                else
                {
                    lock (connections)
                    {
                        connections.Remove(connection);
                    }
                }
            }
        }

        private void HandleWrite(IAsyncResult ar)
        {
            Connection connection = (Connection)ar.AsyncState;

            lock (connection)
            {
                connection.Stream.EndWrite(ar);
            }
        }

        private bool findAndHandleCommands(Connection connection)
        {
            // I feel bad about this. The performance will suck. But, well, it
            // saves some time in implementation currently :\
            // But, hey, we could re-fuck-tor this to be a java-ish
            // NewlineDelimitedExtractCommandStrategy :D
            List<string> command = new List<string>();
            string part = "";
            bool inQuotes = false;
            for (int i = 0; i < connection.RecvBufferCount; i++)
            {
                char c = (char)connection.RecvBuffer[i];

                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if ((c == ' ' || c == '\t') && !inQuotes)
                {
                    if (part != "")
                    {
                        command.Add(part);
                        part = "";
                    }
                }
                else if (c == '\n')
                {
                    if (part != "")
                    {
                        command.Add(part);
                    }

                    i += 1; // move index _behind_ newline
                    Buffer.BlockCopy(connection.RecvBuffer, i,
                            connection.RecvBuffer, 0, connection.RecvBufferCount - i);
                    connection.RecvBufferCount -= i;

                    if (command.Count > 0)
                    {
                        string response = Dispatch(command.ToArray()) + "\n";
                        byte[] responseBuffer = System.Text.Encoding.ASCII.GetBytes(response);
                        connection.Stream.BeginWrite(responseBuffer, 0, responseBuffer.Length, HandleWrite, connection);
                    }

                    return true;
                }
                else
                {
                    part += c;
                }
            }

            return false;
        }

        private class Connection
        {
            public Connection(TcpClient client)
            {
                Client = client;
                Stream = client.GetStream();
                RecvBuffer = new byte[BufferSize];
                RecvBufferCount = 0;
            }

            public TcpClient Client { get; set; }
            public NetworkStream Stream { get; set; }
            public byte[] RecvBuffer { get; set; }
            public int RecvBufferCount { get; set; }
        }
    }
}