using Crawler.NetWork.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit;

namespace Crawler.Spider
{
    public class YouTubeSpider
    {
        private static string youtubeVideoPath = @"F:\Project\video\eu\youtube";
        private static string youtubeUserVideoPath = @"F:\Project\video\eu\youtube\user_video";
        private static string baseVideoUrlFormat = "https://www.youtube.com/watch?v={0}";
        public static List<string> AnalyseVideoList(string url)
        {
            var htmlContent = NetHandle.AccessNetwork(url, false).Item2;
            List<string> childUrls = new List<string>();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            var listNode = htmlDoc.DocumentNode.SelectSingleNode(@"//iron-list[@class='playlist-items yt-scrollbar-dark style-scope ytd-playlist-panel-renderer']");
            var olNode = htmlDoc.DocumentNode.SelectSingleNode(@"//ol[@id='playlist-autoscroll-list']");
            if (olNode == null) return childUrls;

            var liNodes = olNode.Descendants("li");
            foreach (var liNode in liNodes)
            {
                var videoId = liNode.Attributes["data-video-id"].Value;
                var videoUrl = string.Format(baseVideoUrlFormat, videoId);
                Console.WriteLine(childUrls);
                childUrls.Add(videoUrl);
            }
            return childUrls;
        }
        private static void CollectVideoList(string url)
        {
            var htmlContent = NetHandle.AccessNetwork(url, false).Item2;
           
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            var liNode = htmlDoc.DocumentNode.SelectSingleNode(@"//li[@class='author-attribution']");
            var userName = liNode.InnerText.Trim().ToLower();
            if (!Directory.Exists(youtubeUserVideoPath)) Directory.CreateDirectory(youtubeUserVideoPath);
            var currentUserPath = Path.Combine(youtubeUserVideoPath, userName);
            var logPath = Path.Combine(currentUserPath, "video_list.log");
            var childUrls = AnalyseVideoList(url);
            var existUrls = File.ReadAllLines(logPath, Encoding.UTF8);
            foreach (var childUrl in childUrls)
            {
                Console.WriteLine("开始下载--" + childUrl);
                if (existUrls.Contains(childUrl)) continue;
                VideoSpiderTools.YoutubedlDownload(childUrl, currentUserPath, false);
                LogHelper.WriteLogs(childUrl, logPath);
            }
        }
    }
}
