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
        private static string host = "https://v.qq.com";
        private static string formatUrl = "http://c.v.qq.com/vchannelinfo?otype=json&uin={0}&qm=1&pagenum={1}&num=24";
        private static string videoDir = @"F:\Project\video\cn\tencent\video_by_user";
        private static List<string> Analyse(string url, int pageNum, int lowViewCount)
        {
            if (!Directory.Exists(videoDir)) Directory.CreateDirectory(videoDir);
            //url = "http://v.qq.com/vplus/cb5be02aeda6adbbbac790ee1028a77e/videos";
            //http://c.v.qq.com/vchannelinfo?otype=json&uin=cb5be02aeda6adbbbac790ee1028a77e&qm=1&pagenum=3&num=24
            var currenId = url.Substring(url.IndexOf("vplus/") + 6, url.IndexOf("/videos") - url.IndexOf("vplus/") - 6);
            List<string> urls = new List<string>();
            for (int i = 1; i < pageNum + 1; i++)
            {

                var currentUrl = string.Format(formatUrl, currenId, i.ToString());

                var content = NetWorkHandle.GetHtmlContent(currentUrl).Item2;
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
                    var playCountStr = Convert.ToString(singleVideolst["play_count"].Value);
                    var playCount = 0;
                    //1.6万

                    if (playCountStr.Contains("万"))
                    {
                        playCountStr = playCountStr.Replace("万", "");
                        var tempCount = Convert.ToDouble(playCountStr);
                        tempCount = tempCount * 10000;
                        playCount = (int)tempCount;
                    }
                    else
                        playCount = Convert.ToInt32(playCountStr);

                    if (playCount < lowViewCount) continue;
                    var titleObj = singleVideolst["title"];
                    var title = titleObj.Value;
                    urls.Add(childUrl);
                }
                Thread.Sleep(2000);
            }
            return urls;
        }

        private static void DownloadByUser(string url)
        {

            Console.WriteLine("请输入采集的页数：");
            var numStr = Console.ReadLine();
            var pageNum = Convert.ToInt32(numStr);
            Console.WriteLine("请输入最低播放量：");
            var viewCountStr = Console.ReadLine();
            var viewCount = Convert.ToInt32(viewCountStr);
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

            List<string> urls = Analyse(url, pageNum, viewCount);
            foreach (var childUrl in urls)
            {
                Console.WriteLine("下载--" + childUrl);
                try
                {
                    if (existVideos.Contains(childUrl.Trim())) continue;

                    var task = Task.Run(() => {
                        if (VideoSpiderTools.YouGetDownLoad(childUrl, userVideoPath, false))
                        {
                            Console.WriteLine(childUrl + "--下载成功");
                            LogHelper.WriteLogs(childUrl.Trim(), logPath);
                        }
                        else
                        {
                            Console.WriteLine(childUrl + "--下载失败");
                        }
                    });
                    if (!task.Wait(TimeSpan.FromMinutes(3)))
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

        private static void DownloadByList(string url)
        {
           
            var urls = new List<string>();
            Console.WriteLine("开始采集");
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);
            var content = NetWorkHandle.GetHtmlContent(url);
            var aa = content;
            var ulNode = doc.DocumentNode.SelectSingleNode(@"//ul[@class='figure_list']");
            if (ulNode is null) return;
            var liNodes = ulNode.Descendants("li");
            if (liNodes is null) return;
            foreach (var liNode in liNodes)
            {
                var aNode = liNode.Descendants("a").FirstOrDefault();
                if (aNode is null) continue;
                var href = aNode.GetAttributeValue("href", "");
                var childUrl = host + href;
                Console.WriteLine(childUrl);
                urls.Add(childUrl);
            }
            var todayDir = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(todayDir)) Directory.CreateDirectory(todayDir);
            foreach (var childUrl in urls)
            {
                Console.WriteLine("下载--" + childUrl);
                try
                {
                   
                    var task = Task.Run(() => {
                        if (VideoSpiderTools.YouGetDownLoad(childUrl, todayDir, false))
                        {
                            Console.WriteLine(childUrl + "--下载成功");
                            //LogHelper.WriteLogs(childUrl.Trim(), logPath);
                        }
                        else
                        {
                            Console.WriteLine(childUrl + "--下载失败");
                        }
                    });
                    if (!task.Wait(TimeSpan.FromMinutes(3)))
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
        public static void TencentRun(string url)
        {
            if (url.Contains("/vplus/")) DownloadByUser(url);
            else DownloadByList(url);
        }
        
    }
}
