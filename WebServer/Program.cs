using Agile.Helpers;
using Agile.HttpServer;
using Agile.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var opts = new Options
            {
                Backlog = 5,
                Port = 10025,
                Received = Received,
                Started = Started
            };

            var server = new SocketServer(opts);

            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            Console.ReadKey();
        }

        static void Started(string epstr)
        {
            Console.WriteLine(epstr);
        }

        static void Received(Socket client, string str)
        {
            Console.WriteLine(client.LocalEndPoint.ToString());
            Console.WriteLine(str);

            var response = new Response
            {
                HttpVersion = "HTTP/1.1",
                Status = 200,
                Params = new Dictionary<string, string>()
            };

            var request = HttpServer.ParseRequest(str);
            if (request.RawUrl.StartsWith("/"))
            {
                if (request.RawUrl.Length == 1)
                {
                    request.RawUrl = "index.html";
                }
                else
                {
                    request.RawUrl = request.RawUrl.Substring(1);
                    request.RawUrl = request.RawUrl.Replace("/", "\\").Replace(".ui", ".html");
                }
            }

            var filename = request.RawUrl;
            var fullname = System.IO.Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, filename });
            if (!System.IO.File.Exists(fullname))
            {
                fullname = System.IO.Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "404.html" });
            }

            var content = System.IO.File.ReadAllText(fullname);
            var bytes = Encoding.UTF8.GetBytes(content);
            response.Params.Add("Server", "Agile");
            response.Params.Add("Content-Type", "text/html");
            response.Params.Add("Content-Length", bytes.Length.ToString());
            response.Body = content;

            var responsestr = HttpServer.BuildResponse(response);
            var buffer = Encoding.UTF8.GetBytes(responsestr);
            client.Send(buffer);
        }
    }
}
