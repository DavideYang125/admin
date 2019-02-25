using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Crawler.Spider;
using Crawler.SpiderVideo;

namespace Crawler
{
    public class Program
    {
        private static string tencentDomain = "v.qq.com";
        private static string youtubeDomain = "youtube.com";
        private static string bilibiliDomain = "bilibili.com";
        static void Main(string[] args)
        {
            Console.WriteLine("请选择采集类型：");
            Console.WriteLine("1-单个采集");
            Console.WriteLine("2-系列采集(youtube,腾讯,bilibili,youku)");
            
            var typeStr = Console.ReadLine();
            if (typeStr == "1")
            {
                SpiderToolsEntry.SpiderRun();
            }
            else if (typeStr == "2")
            {
                Console.WriteLine("video spider tool,please input url:");
                string url = Console.ReadLine();
                if (url.Contains(tencentDomain))
                {
                    Console.WriteLine("腾讯系列采集");
                    TencentVideoSpider.TencentRun(url);
                }
                else if (url.Contains(youtubeDomain))
                {
                    Console.WriteLine("youtube系列采集");
                    YouTubeSpider.CollectYoutubeVideos(url);
                }
                else if (url.Contains(bilibiliDomain))
                {
                    Console.WriteLine("bilibili系列采集");
                    BilibiliSpider.SpiderByUser(url);
                }
                else
                {
                    Console.WriteLine("输入的链接无效");
                    Console.ReadKey();
                }
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
