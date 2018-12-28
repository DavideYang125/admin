using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Spider
{
    public partial class SpiderToolsEntry
    {
        private static string youtubeVideoPath = @"F:\Project\video\eu\youtube";
        private static string iqiyiVideoPath = @"F:\Project\video\cn\aiqiyi";
        private static string bilibiliVideoPath = @"F:\Project\video\cn\bilibili";
        private static string youkuVideoPath = @"F:\Project\video\cn\youku";
        private static string tencentVideoPath = @"F:\Project\video\cn\tencent";
        private static string yougetVideoPath = @"F:\Project\video\cn\youget";
        private static string sohuVideoPath = @"F:\Project\video\cn\sohu";

        public static void SpiderRun()
        {
            Console.WriteLine("spider tools:youtube-dl,youget");
            Console.WriteLine("please input url;");
            var url = Console.ReadLine();
            url = url.Trim();
            var basePath = youtubeVideoPath;
            if (url.Contains("iqiyi.com"))
            {
                url = url + "\\";
                basePath = iqiyiVideoPath;
            }
            if (url.Contains("bilibili.com")) basePath = bilibiliVideoPath;
            if (url.Contains("youku.com")) basePath = youkuVideoPath;
            if (url.Contains("qq.com")) basePath = tencentVideoPath;
            if (url.Contains("sohu.com")) basePath = sohuVideoPath;
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            var todayDir = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(todayDir)) Directory.CreateDirectory(todayDir);
            var cutEnd = false;
            if (url.EndsWith("@")) cutEnd = true;
            url = url.Replace("@", "").Trim();
            if(url.Contains("qq.com"))
            {
                VideoSpiderTools.YouGetDownLoad(url, todayDir, cutEnd);
            }
            else
            {
                VideoSpiderTools.YoutubedlDownload(url, todayDir, cutEnd);
            }
        }
    }
}
