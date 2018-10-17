using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using System.IO;

namespace ArdSender_WPF
{
	public class GetIP
	{
		static string host = Dns.GetHostName();
		public IPAddress ip = Dns.GetHostEntry(host).AddressList[1];
	}
	public class HttpServer
	{
		HttpListener server = new HttpListener();
		public HttpServer(string ip)
		{
			test(ip);
		}
		public void test(string ip)
		{
			server.Prefixes.Add("http://" + ip + "/");
			server.Start();
		}
			public async void Update(string cpu, string gpu)
			{
					HttpListenerContext context = await server.GetContextAsync();
					HttpListenerRequest request = context.Request;
					HttpListenerResponse response = context.Response;
					string responseStr = "<html><head><meta charset='utf8'></head><body>CPUTemp:" + cpu + "<br> GPUTemp:" + gpu + "</body></html>";
					byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
					response.ContentLength64 = buffer.Length;
					Stream output = response.OutputStream;
					output.Write(buffer, 0, buffer.Length);
					output.Close();
			}
		}
	}

