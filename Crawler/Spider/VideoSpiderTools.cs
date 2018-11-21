using Crawler.IO;
using Crawler.NetWork.Utils;
using Crawler.Spider.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Spider
{
	public class VideoSpiderTools
	{
		public static string YoutubeDlPath = @"D:\Soft\youtubedl\youtube-dl.exe";
		public const string basePath = @"F:\Project\video\cn";
        public static string youtuubeVideoPath = @"F:\Project\video\eu\youtube";
        private static string yougetPath = @"D:\tool\you-get.exe";
        /// <summary>
        /// 第三方工具
        /// </summary>
        /// <param name="toolPath"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDir"></param>
        /// <returns></returns>
        public static string StartToolProcess(string toolPath, string arguments, string workingDir = "")
		{
			var output = string.Empty;
			var startInfo = new ProcessStartInfo(toolPath, arguments)
			{
				WorkingDirectory = workingDir,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};

			var process = Process.Start(startInfo);
			output = process.StandardOutput.ReadToEnd();

			return output;
		}
		/// <summary>
		/// youtube-dl下载
		/// </summary>
		/// <param name="url"></param>
		/// <param name="saveFilePath"></param>
		/// <returns></returns>
		public static bool YoutubedlDownload(string url,string saveFilePath, bool cutEnd = false)
		{
            var tempSavePath = Path.Combine(saveFilePath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempSavePath);
            //检测下载链接
            Console.WriteLine("开始下载:" + url);
			var checkArguments = string.Format("\"{0}\" -j --write-thumbnail", url);//.Replace("&from=ted","")
			var checkOutput = StartToolProcess(YoutubeDlPath, checkArguments, tempSavePath);
			VideoInfo checkVideoInfo = null;
			try
			{
				checkVideoInfo = JsonConvert.DeserializeObject<VideoInfo>(checkOutput);
			}
			catch (Exception ex)
			{
                if(url.Contains("youtube.com"))
                    return false;
			}
			if (checkVideoInfo == null&&url.Contains("youtube.com"))
			{
				var printInfo = string.Format($"未检测到 {url} 下有可用下载链接");
				Console.WriteLine(printInfo);
				return false;
			}
            var videoType = "mp4";
            if (url.Contains("bilibili.com")) videoType = "flv";
			//开始下载
			var arguments = string.Format("\"{0}\" -f \"{1}\" --write-thumbnail --print-json --newline", url, videoType);
            arguments = string.Format("\"{0}\" -f \"{1}\" --write-thumbnail --print-json --newline", url, videoType);
            var output = StartToolProcess(YoutubeDlPath, arguments, tempSavePath);
            var videoPath = FileHandle.GetFirstFileBySuffix(tempSavePath, videoType);
            
            if (string.IsNullOrEmpty(videoPath)) return false;
            var NormalVideoPath = GetNewFilePath(videoPath);
            File.Move(videoPath, NormalVideoPath);
            if(cutEnd) ToolKit.MediaHelper.CutOneSecondForVideo(NormalVideoPath);
            var files = Directory.GetFiles(tempSavePath);
            foreach(var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                var newFilePath = Path.Combine(saveFilePath, fileName);
                File.Move(filePath, newFilePath);
            }
            Directory.Delete(tempSavePath);
			return true;
		}

        public static bool YouGetDownLoad(string url, string saveFilePath, bool cutEnd = false)
        {
            var tempSavePath = Path.Combine(saveFilePath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempSavePath);
            var arguments = url;
            var output = StartToolProcess(yougetPath, arguments, tempSavePath);
            var videoType = "mp4";
            var videoPath = FileHandle.GetFirstFileBySuffix(tempSavePath, videoType);

            if (string.IsNullOrEmpty(videoPath)) return false;
            var NormalVideoPath = GetNewFilePath(videoPath);
            File.Move(videoPath, NormalVideoPath);
            if (cutEnd) ToolKit.MediaHelper.CutOneSecondForVideo(NormalVideoPath);
            var files = Directory.GetFiles(tempSavePath);
            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                var newFilePath = Path.Combine(saveFilePath, fileName);
                File.Move(filePath, newFilePath);
            }
            Directory.Delete(tempSavePath);
            return true;
        }
        /// <summary>
        /// 处理标题
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetNewFilePath(string filePath)
        {
            //var fileNameWithExten = Path.GetFileName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var exten = Path.GetExtension(filePath);
            var dirPath = Path.GetDirectoryName(filePath);
            var newFileName = fileName;
            if (newFileName.Trim().StartsWith("："))
                newFileName = newFileName.Substring(1, newFileName.Length - 1);
            if (newFileName.Contains("-"))
                newFileName = newFileName.Substring(0, newFileName.LastIndexOf("-"));
            return Path.Combine(dirPath, newFileName + exten);
        }

        public virtual bool DownloadFile(string url, string saveFilePath)
		{
			return NetHandle.DownFileMethod(url, saveFilePath);
		}
		/// <summary>
		/// 记录导入情况
		/// </summary>
		/// <param name="recordInfo"></param>
		/// <param name="recordFile"></param>

		public static void RecordFile(string recordInfo, string recordFile = "记录文件", string path = basePath)
		{
			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string filepath = "";
				if (recordFile.Contains("\\"))
				{
					filepath = recordFile;
				}
				else
					filepath = Path.Combine(path, recordFile + ".txt");

				using (StreamWriter sw = new StreamWriter(filepath, true))
				{
					sw.WriteLine(recordInfo);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("记录异常");
			}
		}
		public static string ReplaceQuote(string str)
		{
			str = str.Replace("!", "").Replace("！","").Replace("\"","").Replace("“","")
				.Replace("”","").Replace(".","").Replace("。","").Replace(",","").Replace("，","")
				.Replace("(","").Replace(")","").Replace("（","").Replace("）","").Replace(" ","");
			return str;
		}
	}
    public class SpiderModel
    {
        public string videoUrl;
        public string ImgUrl;
        public string Title;
        public Guid Id;
    }
}
