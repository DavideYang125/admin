using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Spider;
using Utils;


namespace Crawler
{
	public class Program
	{
		static void Main(string[] args)
		{
            YouTubeSpider.AnalyseVideoList("https://www.youtube.com/watch?v=N9r-6v0kxaE&index=47&list=PLA8NZSto-k4Nh5bRtXJsau1mKByeTOkqe");
            TencentVideoSpider.DownloadBuUser();
            return;
            //SpiderToolsEntry.SpiderRun();
            //return;
            SpiderToolsEntry.SpiderRun();
            return;
			string orderStr = string.Empty;
			if (args.Any())
			{
				orderStr = args[0];
			}
			else
			{
				Console.WriteLine(@"9gag采集：输入 gag");
				Console.WriteLine(@"aiqiyi采集：输入 aiqiyi");
				orderStr = Console.ReadLine().Trim().ToLower();
				Console.WriteLine(@"您输入的是:" + orderStr);
			}
			switch (orderStr)
			{
				case "gag":
					GagSpiderRun();
					break;
				case "aiqiyi":
					AiqiyiSpider.GetAiqiyiVideos();
					break;

			}
		}
		public static void GagSpiderRun()
		{
			while (true)
			{
				GagSpider.DownLoadFiles();
				Thread.Sleep(600000);
			}
		}

	}
}
