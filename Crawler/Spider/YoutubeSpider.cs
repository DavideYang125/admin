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
        private static string youkuVideoPath = @"F:\Project\video\cn\youku";
        public static void DownloadSingleYoutube()
        {
            try
            {
                var url = "";
                Console.WriteLine("please input url;");
                url = Console.ReadLine();
                url = url.Trim();
                var basePath = youtubeVideoPath;
                if (url.Contains("iqiyi.com")) basePath = iqiyiVideoPath;
                if (url.Contains("bilibili.com")) basePath = bilibiliVideoPath;
                if (url.Contains("youku.com")) basePath = youkuVideoPath;
                if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                var todayDir = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(todayDir)) Directory.CreateDirectory(todayDir);
                var cutEnd = false;
                if (url.EndsWith("@")) cutEnd = true;
                url = url.Replace("@", "").Trim();
                BaseSpider.YoutubedlDownload(url, todayDir, cutEnd);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }

        }
    }
}
