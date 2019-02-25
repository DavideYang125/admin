using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Crawler.IO;

namespace Crawler.NetWork.Utils
{
	public class NetWorkHandle
	{
		
		public static Tuple<HttpStatusCode, string> GetHtmlContent(string url, string parameter = "", string referer = "")
		{
			Tuple<HttpStatusCode, string> htmlResult = new Tuple<HttpStatusCode, string>(HttpStatusCode.Gone, string.Empty);
			string content = string.Empty;
			try
			{
				var clientHandler = new HttpClientHandler();
				if (clientHandler.SupportsAutomaticDecompression)
				{
					clientHandler.AutomaticDecompression = DecompressionMethods.GZip |													 DecompressionMethods.Deflate;
				}
				using (var httpClient = new HttpClient(clientHandler))
				{
					var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
					if (!string.IsNullOrEmpty(parameter))
					{
						requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
						requestMessage.Content = new StringContent(parameter, Encoding.UTF8, "application/json");
					}
                    httpClient.Timeout = TimeSpan.FromSeconds(25);
					requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.89 Safari/537.36");
                    if(url.Contains("c.v.qq.com")) requestMessage.Headers.Add("host", "c.v.qq.com");

                    if (!string.IsNullOrEmpty(referer)) requestMessage.Headers.Add("Referer", referer);
					var response = httpClient.SendAsync(requestMessage).Result;
					content = response.Content.ReadAsStringAsync().Result;
					htmlResult = new Tuple<HttpStatusCode, string>(response.StatusCode, content);
				}
			}
			catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
			return htmlResult;
		}

		/// <summary>
		/// 获得URL响应返回头
		/// </summary>
		/// <param name="url"></param>
		/// <returns>返回状态码以及Mime类型</returns>
		public static Tuple<HttpStatusCode, string> GetHeaders(string url)
		{
			HttpStatusCode result = default(HttpStatusCode);

			try
			{
				var request = WebRequest.Create(url);
				request.Method = "HEAD";

				var content = "";
				using (var response = request.GetResponse() as HttpWebResponse)
				{
					if (response != null)
					{
						result = response.StatusCode;
						content = response.ContentType;
					}
				}

				return new Tuple<HttpStatusCode, string>(result, content);
			}
			catch (Exception)
			{
				return new Tuple<HttpStatusCode, string>(HttpStatusCode.NotFound, "");
			}
		}

		public static bool DownFileMethod(string url, string path)
		{
			var myWebClient = new WebClient();
			bool isSucess = false;
			if (!string.IsNullOrEmpty(url) && url.Trim().StartsWith("//"))
			{
				url = "http:" + url;
			}
			try
			{
				Thread.Sleep(1000);
				myWebClient.Headers.Set("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

				myWebClient.DownloadFile(url.Trim(), path);
				File.SetAttributes(path, FileAttributes.Normal);
				isSucess = true;
			}
			catch (Exception ex) { }
			return isSucess;
		}

		/// <summary>
		/// 从URL中获取MimeType类型
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string GetMimeTypeFromUrl(string url)
		{
			string contentType = "";
			var request = HttpWebRequest.Create(url) as HttpWebRequest;

			try
			{
				if (request != null)
				{
					var response = request.GetResponse() as HttpWebResponse;
					if (response != null) contentType = response.ContentType;
				}
			}
			catch (Exception)
			{
			}

			return contentType;
		}
		
		public static string GetSuffixFromUrl(string url, string defaultValue = "")
		{
			try
			{
				if (string.IsNullOrEmpty(url)) return defaultValue;
				var lastStr = url.Substring(url.LastIndexOf('/'), url.Length - url.LastIndexOf('/'));
				if (lastStr.Contains("."))
				{
					return lastStr.Substring(lastStr.LastIndexOf('.'), lastStr.Length - lastStr.LastIndexOf('.'));
				}
			}
			catch (Exception)
			{
				return defaultValue;
			}

			return defaultValue;
		}

		public static XDocument GetSpecialHtmlContent(string url)
		{
			var htmlContent = GetHtmlContent(url).Item2;
			if (string.IsNullOrEmpty(htmlContent)) return null;

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(htmlContent);

			XDocument xDoc = DocumentToXDocument(xmlDocument);
			return xDoc;
		}

		private static XDocument DocumentToXDocument(XmlDocument doc)
		{
			return XDocument.Parse(doc.OuterXml);
		}
	}
}
