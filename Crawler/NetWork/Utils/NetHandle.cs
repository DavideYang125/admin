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
	public class NetHandle
	{
		/// <summary>
		/// 访问网站获得状态码和访问后的内容
		/// </summary>
		/// <param name="url"></param>
		/// <param name="checkMimeType">是否检查mimetype类型</param>
		/// <returns></returns>
		public static Tuple<HttpStatusCode, string> AccessNetwork(string url, bool checkMimeType = false, string parameter = "", string referer = "")
		{
			Tuple<HttpStatusCode, string> result = new Tuple<HttpStatusCode, string>(HttpStatusCode.Gone, string.Empty);
			string content = string.Empty;

			try
			{

				var handler = new HttpClientHandler();
				if (handler.SupportsAutomaticDecompression)
				{
					handler.AutomaticDecompression = DecompressionMethods.GZip |
													 DecompressionMethods.Deflate;
				}
				using (var httpClient = new HttpClient(handler))
				{
					var message = new HttpRequestMessage(HttpMethod.Get, url);
					if (!string.IsNullOrEmpty(parameter))
					{
						message = new HttpRequestMessage(HttpMethod.Post, url);
						message.Content = new StringContent(parameter, Encoding.UTF8, "application/json");
					}
					httpClient.Timeout = TimeSpan.FromSeconds(30);
					message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.89 Safari/537.36");
					if (!string.IsNullOrEmpty(referer)) message.Headers.Add("Referer", referer);
					var response = httpClient.SendAsync(message).Result;
					content = response.Content.ReadAsStringAsync().Result;
					result = new Tuple<HttpStatusCode, string>(response.StatusCode, content);
				}
			}
			catch (Exception) { }
			return result;
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

		/// <summary>
		/// 下载文件
		/// </summary>
		/// <param name="url">网络下载路径</param>
		/// <param name="path">存放文件路径</param>
		/// <param name="articleLink"></param>
		/// <returns>是否下载成功</returns>
		public static bool DownFileMethod(string url, string path, string articleLink = "")
		{
			var myWebClient = new WebClient();
			bool isDown = false;
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
				isDown = true;
			}
			catch (Exception ex) { }
			return isDown;
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

		/// <summary>
		/// 从URL中获得文件后缀名
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string GetFileSuffixFromUrl(string url, string defaultValue = "")
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

		/// <summary>
		/// 由于有的网页是经过压缩处理的所以不能直接使用URL来获得XML内容
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static XDocument EscapeSpecialHtmlContent(string url)
		{
			var htmlContent = AccessNetwork(url, false).Item2;
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
