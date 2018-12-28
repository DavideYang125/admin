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
            Console.WriteLine("请选择采集类型：");
            Console.WriteLine("1-单个采集");
            Console.WriteLine("2-youtube系列采集");
            Console.WriteLine("3-腾讯系列采集");
            var typeStr = Console.ReadLine();
            if (typeStr == "1")
            {
                SpiderToolsEntry.SpiderRun();
            }
            else if (typeStr == "2")
            {
                YouTubeSpider.CollectYoutubeVideos();
            }
            else if (typeStr == "3")
            {
                TencentVideoSpider.DownloadByUser();                
            }
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
