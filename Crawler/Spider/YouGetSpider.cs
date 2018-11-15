using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Spider
{
    public class YouGetSpider
    {
        private static string tencentVideoPath = @"F:\Project\video\cn\tencent";
        private static string iqiyiVideoPath = @"F:\Project\video\cn\aiqiyi";
        private static string yougetVideoPath = @"F:\Project\video\cn\youget";
        public static void StartYouGetSpider()
        {
            var url = "";
            Console.WriteLine("please input url;");
            url = Console.ReadLine();
            var basePath = yougetVideoPath;
            if (url.Contains("qq.com")) basePath = tencentVideoPath;
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            var todayDir = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(todayDir)) Directory.CreateDirectory(todayDir);
            var cutEnd = false;
            if (url.EndsWith("@")) cutEnd = true;
            url = url.Replace("@", "").Trim();
            BaseSpider.YouGetDownLoad(url, todayDir, cutEnd);
        }  
    }
}
