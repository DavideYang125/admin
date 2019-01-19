using Crawler.NetWork.Utils;
using Crawler.Spider;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ToolKit;

namespace Crawler.SpiderVideo
{
    public class BilibiliSpider
    {
        private static string basePath = @"F:\Project\video\cn\bilibili";
        
        private static string videoDir = @"F:\Project\video\cn\bilibili\video_by_user";

        private static List<string> Analyse(string url)
        {
            List<string> urls = new List<string>();
            //url = "https://space.bilibili.com/25911961/video";
            var code = url.Replace("https://space.bilibili.com/", "").Split('/')[0];
            //https://space.bilibili.com/ajax/member/getSubmitVideos?mid=25911961&pagesize=30&tid=0&page=1&keyword=&order=pubdate
            var jsonUrl = $"https://space.bilibili.com/ajax/member/getSubmitVideos?mid={code}&pagesize=30&tid=0&page=1&keyword=&order=pubdate";
            var jsonContent = NetWorkHandle.GetHtmlContent(jsonUrl).Item2;
            jsonContent = Regex.Unescape(jsonContent);
            dynamic jsonObj = JsonConvert.DeserializeObject(jsonContent);
            var data = jsonObj.data;
            var vlists = data.vlist;
            foreach (var vlist in vlists)
            {
                string title = vlist.title;
                string aid = vlist.aid;
                var childUrl = "https://www.bilibili.com/video/av" + aid;
                urls.Add(childUrl);
            }

            return urls;
        }

        public static void SpiderByUser(string url)
        {
            Console.WriteLine("开始采集");
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);
            var userNameNode = doc.DocumentNode.SelectSingleNode(@"//title");
            if (userNameNode is null)
            {
                Console.WriteLine("无法获取username,停止采集");
                return;
            }
            var userName = userNameNode.InnerText.Trim();
            if (!userName.Contains("的个人空间"))
            {
                Console.WriteLine("无法获取username,停止采集");
                return;
            }
            userName = userName.Substring(0, userName.IndexOf("的个人空间"));
            Console.WriteLine("user name:" + userName);
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            if (!Directory.Exists(videoDir)) Directory.CreateDirectory(videoDir);
            var userVideoPath = Path.Combine(videoDir, userName);
            var logPath = Path.Combine(userVideoPath, "video_list.log");
            var existVideos = new List<string>();
            if (!Directory.Exists(userVideoPath)) Directory.CreateDirectory(userVideoPath);
            else
            {
                if (File.Exists(logPath)) existVideos = File.ReadLines(logPath).ToList();
            }

            List<string> urls = Analyse(url);
            foreach (var childUrl in urls)
            {
                Console.WriteLine("下载--" + childUrl);
                try
                {
                    if (existVideos.Contains(childUrl.Trim())) continue;

                    var task = Task.Run(() => {
                        if (VideoSpiderTools.YoutubedlDownload(childUrl, userVideoPath, false))
                        {
                            Console.WriteLine(childUrl + "--下载成功");
                            LogHelper.WriteLogs(childUrl.Trim(), logPath);
                        }
                        else
                        {
                            Console.WriteLine(childUrl + "--下载失败");
                        }
                    });
                    if (!task.Wait(TimeSpan.FromMinutes(5)))
                    {
                        Console.WriteLine(childUrl + "--超时退出，下载失败");
                    }
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
