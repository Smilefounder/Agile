using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Helpers
{
    public class SocketHelper
    {
        public delegate void ClientConnectedEventHandler(string address);

        public delegate void DataReceivedEventHandler(byte[] buffer, int length);

        public event ClientConnectedEventHandler ClientConnected;

        public event DataReceivedEventHandler DataReceived;

        private SocketHelper()
        {

        }

        public SocketHelper(string ipAddress, int port)
        {
            var backlog = 10;
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(backlog);
            listener.BeginAccept(new AsyncCallback(Accept), listener);
        }

        private byte[] buffer = new byte[1024];

        private void Accept(IAsyncResult asyncResult)
        {
            var listener = asyncResult.AsyncState as Socket;
            var client = listener.EndAccept(asyncResult);

            if (ClientConnected != null)
            {
                ClientConnected(client.LocalEndPoint.ToString());
            }

            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), client);
            listener.BeginAccept(new AsyncCallback(Accept), listener);
        }

        private void Receive(IAsyncResult asyncResult)
        {
            var client = asyncResult.AsyncState as Socket;
            var length = client.EndReceive(asyncResult);
            if (length > 0)
            {
                if (DataReceived != null)
                {
                    DataReceived(buffer, length);
                }
            }

            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), client);
        }
    }
}
