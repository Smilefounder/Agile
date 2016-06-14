using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Agile.SocketServer
{
    public class SocketServer
    {
        private Options _opts;

        public SocketServer(Options opts)
        {
            _opts = opts;
        }

        public void Start()
        {
            var ipep = new IPEndPoint(IPAddress.Any, _opts.Port);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipep);
            server.Listen(_opts.Backlog);

            server.BeginAccept(Accept, server);
            if (_opts.Started != null)
            {
                _opts.Started.Invoke(server.LocalEndPoint.ToString());
            }
        }

        private byte[] _buffer = new byte[1024];

        public void Accept(IAsyncResult ar)
        {
            var server = (Socket)ar.AsyncState;
            var client = server.EndAccept(ar);

            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Receive, client);
            server.BeginAccept(Accept, server);
        }

        public void Receive(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;
            var count = client.EndReceive(ar);
            if (count > 0)
            {
                if (_opts.Received != null)
                {
                    var str = Encoding.UTF8.GetString(_buffer, 0, count);
                    _opts.Received.Invoke(client, str);
                }
            }

            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Receive, client);
        }
    }

    public delegate void StartedEventHandler(string epstr);

    public delegate void ReceivedEventHandler(Socket client, string str);

    public class Options
    {
        public int Backlog { get; set; }

        public int Port { get; set; }

        public StartedEventHandler Started { get; set; }

        public ReceivedEventHandler Received { get; set; }
    }
}
