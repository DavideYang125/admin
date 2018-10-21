using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Spider
{
    class YoutubeSpider
    {
        private static string youtubeVideoPath = @"F:\Project\video\eu\youtube";
        private static string iqiyiVideoPath = @"F:\Project\video\cn\aiqiyi";
        private static string bilibiliVideoPath = @"F:\Project\video\cn\bilibili";
        public static void DownloadSingleYoutube()
        {
            try
            {
                var url = "";
                Console.WriteLine("please input url;");
                url = Console.ReadLine();
                var basePath = youtubeVideoPath;
                if (url.Contains("iqiyi.com")) basePath = iqiyiVideoPath;
                if (url.Contains("bilibili.com")) basePath = bilibiliVideoPath;
                var todayDir = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(todayDir)) Directory.CreateDirectory(todayDir);

                BaseSpider.YoutubedlDownload(url, todayDir);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }

        }
    }
}
