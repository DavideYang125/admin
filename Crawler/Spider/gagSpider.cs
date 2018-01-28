using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crawler.NetWork.Utils;
using HtmlAgilityPack;

namespace Crawler.Spider
{
	public class GagSpider
	{
		public static string basePath = @"F:\video\9gag";
		public static string existsFileName = "exists";
		public static string existsVideoPath = @"F:\video\9gag\exists.txt";
		/// <summary>
		/// 记录导入情况,保存在路径d:\tool\yww\record下
		/// </summary>
		/// <param name="Record"></param>
		/// <param name="RecordFileName"></param>

		public static void RecordFile(string Record, string RecordFileName = "记录文件", string path = @"F:\video\9gag")
		{
			try
			{
				if (Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string filepath = Path.Combine(path, RecordFileName + ".txt");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				using (StreamWriter sw = new StreamWriter(filepath, true))
				{
					sw.WriteLine(Record);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("记录异常");
			}
		}
		/// <summary>
		/// 根据文本文件获取每行数据放入list
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static List<string> GetTxtList(string path)
		{
			List<string> list = new List<string>();
			using (StreamReader sr = new StreamReader(path))
			{
				string line;

				while ((line = sr.ReadLine()) != null)
				{
					list.Add(line);
				}
			}
			return list;
		}
		public static void DownLoadFiles()
		{
			if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
			if (!File.Exists(existsVideoPath)) File.Create(existsVideoPath);
			var results = SingleSpider();
			foreach (var item in results)
			{
				var id = item.Id.ToString();
				var title = item.Title;
				if (string.IsNullOrEmpty(title))
				{
					title = DateTime.Now.ToString();
				}
				var url = item.videoUrl;
				var videoName = id + ".mp4";
				var imgName = "";
				if (!string.IsNullOrEmpty(item.ImgUrl))
				{
					var imgUrl = item.ImgUrl;
					Console.WriteLine($"开始下载--{item.ImgUrl}");
					imgName = id + ".jpg";
					var imgFilePath = Path.Combine(basePath, imgName);
					if (NetHandle.DownFileMethod(imgUrl, imgFilePath))
					{
						Console.WriteLine($"下载--{item.ImgUrl}--成功");
					}
					else
					{
						Console.WriteLine($"下载--{item.ImgUrl}--失败");
					}
				}

				var videoFilePath = Path.Combine(basePath, videoName);

				if (NetHandle.DownFileMethod(url, videoFilePath))
				{
					Console.WriteLine($"下载--{item.videoUrl}--成功");
					RecordFile(id, RecordFileName: existsFileName, path: basePath);
					RecordFile(title, RecordFileName: existsFileName, path: basePath);
				}
				else
				{
					Console.WriteLine($"下载--{item.videoUrl}--失败");
				}
				Console.WriteLine($"{title}--完成");
			}
		}


		public static List<SpiderModel> SingleSpider()
		{
			var result = new List<SpiderModel>();
			try
			{
				Console.WriteLine("开始分析网页");
				
				var gagUrl = "https://9gag.com/video";
				var htmlContent = NetHandle.AccessNetwork(gagUrl).Item2;
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlContent);
				var articles = htmlDoc.DocumentNode.SelectNodes(@"//article");
				var nowTime = DateTime.Now;
				int i = 1;
				foreach (var item in articles)
				{
					var spidermodel = new SpiderModel();
					var add_time = nowTime.AddMinutes(-4 * i);

					var title = item.ChildNodes[1].InnerText.ToString().Trim();
					var existsList = GetTxtList(existsVideoPath);
					if (existsList.Contains(title))
					{
						Console.WriteLine($"{title}--已存在");
						continue;
					}
					//if()
					var videoUrl = item.ChildNodes[3].ChildNodes[1].ChildNodes[1].GetAttributeValue("data-mp4", "");
					var imgUrl = item.ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[1].GetAttributeValue("poster", "");
					// data-mp4  poster  Regex.Replace(str, @"\s+", " ");
					if (string.IsNullOrEmpty(videoUrl))
					{
						continue;
					}
					spidermodel.Title = title;
					spidermodel.videoUrl = videoUrl;
					spidermodel.ImgUrl = imgUrl;
					spidermodel.Id = Guid.NewGuid();
					result.Add(spidermodel);
					Console.WriteLine($"{title}--添加成功");

				}
			}
			catch (Exception ex)
			{
				return result;
			}
			Console.WriteLine("网页分析完毕");
			return result;
		}
	}
}
