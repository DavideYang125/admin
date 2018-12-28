using Crawler.NetWork.Utils;
using HtmlAgilityPack;
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

namespace Crawler.Spider
{
    public class YouTubeSpider
    {
        private static string youtubeVideoPath = @"F:\Project\video\eu\youtube";
        private static string youtubeUserVideoPath = @"F:\Project\video\eu\youtube\user_video";
        private static string baseVideoUrlFormat = "https://www.youtube.com/watch?v={0}";
        private static List<string> AnalyseVideoList(string url)
        {
            var htmlContent = NetHandler.GetHtmlContent(url).Item2;
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
                Console.WriteLine(videoUrl);
                childUrls.Add(videoUrl);
            }
            return childUrls;
        }
        public static void CollectYoutubeVideos()
        {
            string url = "";
            Console.WriteLine("please input youtube url:");
            url = Console.ReadLine();
            Console.WriteLine("开始采集");
            var htmlContent = NetHandler.GetHtmlContent(url).Item2;
            var userName = "";
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            var liNode = htmlDoc.DocumentNode.SelectSingleNode(@"//li[@class='author-attribution']");
            if(liNode is null)
            {
                var titleSpanNode = htmlDoc.DocumentNode.SelectSingleNode(@"//meta[@name='title']");
                userName = titleSpanNode.GetAttributeValue("content", "");// titleSpanNode.InnerText.Trim().ToLower();
            }
            else
                userName = liNode.InnerText.Trim().ToLower();
            userName = HttpUtility.HtmlDecode(userName);
            userName = userName.Replace(" ", "_").Replace(" ", "_");
            if (!Directory.Exists(youtubeUserVideoPath)) Directory.CreateDirectory(youtubeUserVideoPath);
            var currentUserPath = Path.Combine(youtubeUserVideoPath, userName);
            if (!Directory.Exists(currentUserPath)) Directory.CreateDirectory(currentUserPath);
            var logPath = Path.Combine(currentUserPath, "video_list.log");
            if (!File.Exists(logPath)) File.WriteAllText(logPath, "", Encoding.UTF8);
            var childUrls = new List<string>();
            
            if(new Regex("/user/[^/]+/video").IsMatch(url))
            {
                childUrls = AnalyseVideoUrlListByUserVideoUrl(url);
            }
            else
                childUrls = AnalyseVideoList(url);
            var existUrls = File.ReadAllLines(logPath, Encoding.UTF8);
            foreach (var childUrl in childUrls)
            {
                Console.WriteLine("开始下载--" + childUrl);
                if (existUrls.Contains(childUrl)) continue;
                VideoSpiderTools.YoutubedlDownload(childUrl, currentUserPath, false);
                LogHelper.WriteLogs(childUrl, logPath);
                Thread.Sleep(2000);
            }
        }
      
        private static List<string> AnalyseVideoUrlListByUserVideoUrl(string url)
        {
            var childUrls = new List<string>();
            var htmlContent = NetHandler.GetHtmlContent(url).Item2;
            if (string.IsNullOrEmpty(htmlContent)) return childUrls;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            var htmlNodes = htmlDoc.DocumentNode.SelectNodes(@"//a[@class='yt-uix-sessionlink yt-uix-tile-link  spf-link  yt-ui-ellipsis yt-ui-ellipsis-2']");
            if (htmlNodes == null) return childUrls;
            foreach (var htmlNode in htmlNodes)
            {
                childUrls.Add("https://www.youtube.com/" + htmlNode.GetAttributeValue("href", ""));
            }
            return childUrls;
        }
    }
}
