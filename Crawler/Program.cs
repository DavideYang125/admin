using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Spider;

namespace Crawler
{
	public class Program
	{
		static void Main(string[] args)
		{
            YouTubeSpider.CollectYoutubeVideos();
            return;
            TencentVideoSpider.DownloadByUser();
            return;
            SpiderToolsEntry.SpiderRun();
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
