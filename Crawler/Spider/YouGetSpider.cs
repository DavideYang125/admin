using System;
using System.Collections.Generic;
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
            var baseDir = yougetVideoPath;
            if (url.Contains("qq.com")) baseDir = tencentVideoPath;
            BaseSpider.YouGetDownLoad(url, baseDir);
        }  
    }
}
