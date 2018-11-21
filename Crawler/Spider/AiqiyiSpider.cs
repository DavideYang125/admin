using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler.Spider
{
	public class AiqiyiSpider
	{
		public static string fileDir = @"F:\Project\video\cn\aiqiyi";
		public static string fileinfoLog = @"F:\Project\video\cn\aiqiyi\aiqiyi_video.log";

		public static void GetAiqiyiVideos()
		{
			//http://list.iqiyi.com/www/22/29116-------------11-1-2-iqiyi-1-.html   //搞笑短片
			//http://list.iqiyi.com/www/22/29116-------------11-2-2-iqiyi-1-.html


			//http://list.iqiyi.com/www/22/22169-------------11-1-2-iqiyi-1-.html   //欢乐精选
			//http://list.iqiyi.com/www/22/22169-------------11-2-2-iqiyi-1-.html

			//搞笑短片
			var gaoxiaoFormat = "http://list.iqiyi.com/www/22/29116-------------4-{0}-2-iqiyi-1-.html";
			Console.WriteLine("开始下载爱奇艺_搞笑短片");
			for (int i = 1; i < 11; i++)
			{
				Console.WriteLine($"开始下载第 {i.ToString()} 页");
				var url = string.Format(gaoxiaoFormat, i.ToString());
				Console.WriteLine(url);
				Analyze(url);
				Console.WriteLine($"第 {i.ToString()} 页下载完成");
			}

			//欢乐精选
			var huanleFormat = "http://list.iqiyi.com/www/22/22169-------------4-{0}-2-iqiyi-1-.html";
			//http://list.iqiyi.com/www/22/22169-------------4-1-2-iqiyi-1-.html
			Console.WriteLine("开始下载爱奇艺_欢乐精选");
			for (int i = 1; i < 11; i++)
			{
				Console.WriteLine($"开始下载第 {i.ToString()} 页");
				var url = string.Format(huanleFormat, i.ToString());
				Console.WriteLine(url);
				Analyze(url);
				Console.WriteLine($"第 {i.ToString()} 页下载完成");
			}
		}

		public static void Analyze(string url)
		{
			var fileinfos = File.ReadAllLines(fileinfoLog, Encoding.UTF8);

			//class="wrapper-piclist"
			var todayDir = Path.Combine(fileDir, DateTime.Now.ToString("yyyyMMdd"));
			if (!Directory.Exists(todayDir)) Directory.CreateDirectory(todayDir);
			HtmlWeb web = new HtmlWeb();
			var doc = web.Load(url);
			var divNode = doc.DocumentNode.SelectSingleNode(@"//div[@class='wrapper-piclist']");
			var liNodes = divNode.Descendants("li");


			foreach (var liNode in liNodes)
			{
				var aNode = liNode.Descendants("a").FirstOrDefault();
				if (aNode == null) continue;
				var title = aNode.GetAttributeValue("title", "").Trim();
				title = VideoSpiderTools.ReplaceQuote(title);
				if (string.IsNullOrEmpty(title)) title = Guid.NewGuid().ToString();
				if (fileinfos.Contains(title))
				{
					Console.WriteLine(title+" 已存在");
					continue;
				} 
				//var tempforder = Path.Combine(todayDir,title);
				//if (Directory.Exists(tempforder)) continue;
				//Directory.CreateDirectory(tempforder);
				Console.WriteLine("获取标题：" + title);
				var href = aNode.GetAttributeValue("href", "").Trim();
				if (string.IsNullOrEmpty(href)) continue;
				//http://www.iqiyi.com/v_19rre87l8k.html#vfrm=2-4-0-1
				var childurl = href.Split('#')[0];
				//var path = Path.Combine(todayDir, title + ".mp4");
				if (VideoSpiderTools.YoutubedlDownload(childurl, todayDir))
				{
					VideoSpiderTools.RecordFile(title, fileinfoLog);
					Console.WriteLine(title + " 下载完成");
				}
				else
				{
					Console.WriteLine(title + " 下载失败");
				}
			}
		}
	}
}
