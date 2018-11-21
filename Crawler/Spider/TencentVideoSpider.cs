using Crawler.NetWork.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolKit;

namespace Crawler.Spider
{
    public class TencentVideoSpider
    {
        private static string basePath = @"F:\Project\video\cn\tencent";
        private static string formatUrl = "http://c.v.qq.com/vchannelinfo?otype=json&uin={0}&qm=1&pagenum={1}&num=24";
        private static string videoDir = @"F:\Project\video\cn\tencent\video_by_user";
        private static List<string> Analyse(string url, int pageNum, List<string> existVideos, string logPath)
        {
            if (!Directory.Exists(videoDir)) Directory.CreateDirectory(videoDir);
            //url = "http://v.qq.com/vplus/cb5be02aeda6adbbbac790ee1028a77e/videos";
            //http://c.v.qq.com/vchannelinfo?otype=json&uin=cb5be02aeda6adbbbac790ee1028a77e&qm=1&pagenum=3&num=24
            var currenId = url.Substring(url.IndexOf("vplus/") + 6, url.IndexOf("/videos") - url.IndexOf("vplus/") - 6);
            List<string> urls = new List<string>();
            for (int i = 1; i < pageNum + 1; i++)
            {

                var currentUrl = string.Format(formatUrl, currenId, i.ToString());

                var content = NetHandle.AccessNetwork(currentUrl).Item2;
                if (string.IsNullOrEmpty(content)) continue;
                content = content.Trim().Replace("QZOutputJson=", "");
                content = content.Substring(0, content.Length - 1);
                dynamic infoObj = JsonConvert.DeserializeObject(content);
                var videolst = infoObj["videolst"];
                if (!videolst.HasValues)
                {
                    return urls;
                }

                foreach (var singleVideolst in videolst)
                {
                    var childUrlObj = singleVideolst["url"];
                    var childUrl = childUrlObj.Value;
                    Console.WriteLine(childUrl);
                    var titleObj= singleVideolst["title"];
                    var title= titleObj.Value;
                    if (existVideos.Contains(title)) continue;
                    LogHelper.WriteLogs(title, logPath);
                    urls.Add(childUrl);
                }
                Thread.Sleep(2000);
            }
            return urls;
        }

        public static void DownloadBuUser()
        {
            Console.WriteLine("tencent video采集器,video by user,请输入url:");
            string url = Console.ReadLine();
            Console.WriteLine("请输入采集的页数：");
            var numStr = Console.ReadLine();
            var pageNum = Convert.ToInt32(numStr);

            Console.WriteLine("开始采集");
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);
            var userNameNode = doc.DocumentNode.SelectSingleNode(@"//span[@id='userInfoNick']");
            if (userNameNode is null)
            {
                Console.WriteLine("无法获取username,停止采集");
                return;
            }
            var userName = userNameNode.InnerText.Trim();
            Console.WriteLine("user name:" + userName);
            var userVideoPath = Path.Combine(videoDir, userName);
            var logPath = Path.Combine(userVideoPath, "video_list.log");
            var existVideos = new List<string>();
            if (!Directory.Exists(userVideoPath)) Directory.CreateDirectory(userVideoPath);
            else
            {
                if (File.Exists(logPath)) existVideos = File.ReadLines(logPath).ToList();
            }
            
            List<string> urls = Analyse(url, pageNum, existVideos, logPath);
            foreach (var childUrl in urls)
            {
                Console.WriteLine("下载--" + childUrl);
                try
                {
                    VideoSpiderTools.YouGetDownLoad(childUrl, userVideoPath, false);
                    Console.WriteLine(childUrl + "--下载成功");
                    Thread.Sleep(2000);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }              
            }
        }
    }
}
