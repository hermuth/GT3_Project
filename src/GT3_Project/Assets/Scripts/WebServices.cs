using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace UnityEngine.Extensions.WebServices
{
	public class HttpServer
	{
		public int state;

		private readonly HttpListener httpListener = new HttpListener();
		private readonly Func<HttpListenerRequest, string> responderMethod;

		private string htmlTemplate = "";

		public HttpServer(string newHtmlTemplate, params string[] prefixes)
		{
			htmlTemplate = newHtmlTemplate;

			if (!HttpListener.IsSupported)
				throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

			if (prefixes == null || prefixes.Length == 0)
				throw new ArgumentException("prefixes");

			if (newHtmlTemplate == null)
				throw new ArgumentException("method");

			foreach (string s in prefixes)
				httpListener.Prefixes.Add(s);

			responderMethod = HtmlResponse;
			httpListener.Start();
        }

		public void Run()
		{
			ThreadPool.QueueUserWorkItem((o) =>
			{
				try
				{
					while (httpListener.IsListening)
					{
                         ThreadPool.QueueUserWorkItem((c) =>
						 {
							 HttpListenerContext context = c as HttpListenerContext;

                             try
							 {
                                 string responseString = this.responderMethod(context.Request);
                                 byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                                 context.Response.ContentLength64 = buffer.Length;
                                 context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                             }
							 catch {}
                             finally
							 {
                                 context.Response.OutputStream.Close();
                             }
                         }, httpListener.GetContext());
                    }
                }
				catch {}
            });
        }

		public void Stop()
		{
             httpListener.Stop();
             httpListener.Close();
        }

		private string HtmlResponse(HttpListenerRequest request)
		{
			return htmlTemplate;
		}
	}

    public class WSServer
	{
		private readonly WebSocketServer webSocketServer;

		//Nested Class
		public class WSService : WebSocketBehavior
		{
			public static RemoteController callbackInstance;
			public static CarManager carManager;

			protected override void OnOpen()
			{
				RemoteController.instance.statusData = "connection open";
				RemoteController.instance.updateStatus = true;
				RemoteController.instance.controllerConnected = true;
			}

			protected override void OnClose(CloseEventArgs e)
			{
				RemoteController.instance.statusData = "connection closed";
				RemoteController.instance.updateStatus = true;
				RemoteController.instance.controllerConnected = false;
			}

			protected override void OnMessage(MessageEventArgs e)
			{
				string type = Regex.Match(e.Data, "^...").Groups[0].Value;

				switch (type) {
					case "dat":
						break;
					case "acc":
					{
						string regexPattern = "^acc\\((-?.*)\\)";
						float value = (float)Convert.ToDouble(Regex.Match(e.Data, regexPattern).Groups[1].Value);
						RemoteController.instance.steerV += value;

						break;
					}
					case "req":
					{
						string regexPattern = "^req\\((.*)\\)";
						string request = Regex.Match(e.Data, regexPattern).Groups[1].Value;

						switch (request)
						{
							case "speed":
							{
								Send("speed(" + carManager.Speed.ToString("0") + ")");
								break;
							}
							case "durability":
							{
								Send("durability(" + carManager.Durability.ToString("0") + ")");
								break;
							}
							default:
								break;
						}

						break;
					}
					case "rot":
					{
						RemoteController.instance.updateOrientation = true;
						RemoteController.instance.orientationData = e.Data;
						break;
					}
					default:
						break;
				}
            }
        }

		public WSServer(int p_socketPort)
		{
            webSocketServer = new WebSocketServer(p_socketPort);
			WSService.carManager = RemoteController.instance.carManager;
			webSocketServer.AddWebSocketService<WSService>("/");
        }

        public void Run()
		{
            webSocketServer.Start();
        }

        public void Stop()
		{
            webSocketServer.Stop();
        }
    }
}
