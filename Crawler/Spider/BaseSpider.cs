﻿using Crawler.NetWork.Utils;
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
	public class BaseSpider
	{
		public static string YoutubeDlPath = @"D:\Soft\youtubedl\youtube-dl.exe";
		public const string basePath = @"F:\Project\video\cn";

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
		public static bool YoutubedlDownload(string url,string saveFilePath)
		{
			//检测下载链接
			var checkArguments = string.Format("\"{0}\" -j --write-thumbnail", url);//.Replace("&from=ted","")
			var checkOutput = StartToolProcess(YoutubeDlPath, checkArguments, saveFilePath);
			VideoInfo checkVideoInfo = null;
			try
			{
				checkVideoInfo = JsonConvert.DeserializeObject<VideoInfo>(checkOutput);
			}
			catch (Exception ex)
			{
				return false;
			}
			if (checkVideoInfo == null)
			{
				var hint = string.Format($"未检测到 {url} 下有可用下载链接");
				Console.WriteLine(hint);
				return false;
			}
			//开始下载
			var arguments = string.Format("\"{0}\" -f \"mp4\" --write-thumbnail --print-json --newline", url);
			var output = StartToolProcess(YoutubeDlPath, arguments, saveFilePath);
			if (File.Exists(saveFilePath)) return false;
			return true;
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
}