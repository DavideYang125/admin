using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace Crawler.IO
{
	public static class FileHandle
	{
		private static object _lock = new object();

		/// <summary>
		/// 根据后缀名查找文件名称，默认返回符合条件的第一个文件名称
		/// </summary>
		/// <param name="mediaPath">资源文件路径, 一般获取media对象后，可以通过media.mediaPath() 或者 GetMediaFilePath来获取路径</param>
		/// <param name="parameters">需搜索的后缀名参数</param>
		/// <param name="type">是否查找data目录</param>
		/// <returns>返回文件名称或者空字符串</returns>
		public static string SearchFileNameBySuffix(string mediaPath, bool returnFullPath, string[] parameters,
			string type = "data", string searchWord = "", string exceptWord = "")
		{
			var mediaDirectory = mediaPath;
			if (type == "data")
			{
				mediaDirectory = Path.Combine(mediaPath, "data");
			}

			if (Directory.Exists(mediaDirectory))
			{
				var files = Directory.GetFiles(mediaDirectory);

				var subFile = (from file in files
							   let name = file.Substring(file.LastIndexOf('.') + 1)
							   where parameters.Contains(name.ToLower())
							   select file.Substring(file.LastIndexOf("\\") + 1)).ToList();

				var fileName = "";
				if (string.IsNullOrEmpty(searchWord))
				{
					if (!string.IsNullOrEmpty(exceptWord))
						fileName = subFile.FirstOrDefault(m => !m.ToLower().Contains(exceptWord.ToLower()));
					else
						fileName = subFile.FirstOrDefault();
				}
				else
				{
					fileName = subFile.FirstOrDefault(m => m.ToLower().Contains(searchWord.ToLower()));
				}
				if (returnFullPath && !string.IsNullOrEmpty(fileName))
				{
					return Path.Combine(mediaDirectory, fileName);
				}
				return fileName;
			}
			return "";
		}

		/// <summary>
		/// 获取文件夹下的第一个指定文件
		/// </summary>
		/// <param name="directory">文件夹路径 </param>
		/// <param name="fileSuffix">文件后缀名 不含.  </param>
		/// <param name="searchWord">路径中包含的字符</param>
		/// <param name="exceptWord">路径中排除的字符 </param>
		/// <returns></returns>
		public static string GetFirstFileBySuffix(string directory, string fileSuffix, string searchWord = "",
			string exceptWord = "")
		{
			if (!Directory.Exists(directory)) return "";
			var files = Directory.GetFiles(directory);
			var subFile = (from file in files
						   let name = file.Substring(file.LastIndexOf('.') + 1)
						   where name.ToLower() == fileSuffix.ToLower()
						   select file).ToList();

			if (string.IsNullOrEmpty(searchWord))
			{
				if (!string.IsNullOrEmpty(exceptWord))
					return subFile.FirstOrDefault(m => !m.ToLower().Contains(exceptWord.ToLower()));
				else return subFile.FirstOrDefault();
			}
			else
			{
				return subFile.FirstOrDefault(m => m.ToLower().Contains(searchWord.ToLower()));
			}
		}

		/// <summary>
		/// 获取文件夹下的第一个指定文件
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="fileSuffix"></param>
		/// <param name="searchWord"></param>
		/// <param name="exceptWord"></param>
		/// <returns></returns>
		public static string GetFirstFileBySuffix(string directory, string[] fileSuffix, string searchWord = "",
			string exceptWord = "")
		{
			if (!Directory.Exists(directory)) return "";
			var files = Directory.GetFiles(directory);
			var subFile = (from file in files
						   let name = file.Substring(file.LastIndexOf('.') + 1)
						   where fileSuffix.Contains(name.ToLower())
						   select file).ToList();

			if (string.IsNullOrEmpty(searchWord))
			{
				if (!string.IsNullOrEmpty(exceptWord))
					return subFile.FirstOrDefault(m => !m.ToLower().Contains(exceptWord.ToLower()));
				else return subFile.FirstOrDefault();
			}
			else
			{
				return subFile.FirstOrDefault(m => m.ToLower().Contains(searchWord.ToLower()));
			}
		}

		/// <summary>
		/// 获取文件夹下的指定文件列表
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="fileSuffix"></param>
		/// <param name="searchWord"></param>
		/// <param name="exceptWord"></param>
		/// <returns></returns>
		public static List<string> GetFilesBySuffix(string directory, string[] fileSuffix, string searchWord = "",
			string exceptWord = "")
		{
			if (!Directory.Exists(directory)) return new List<string>();
			var files = Directory.GetFiles(directory);
			var subFile = (from file in files
						   let name = file.Substring(file.LastIndexOf('.') + 1)
						   where fileSuffix.Contains(name.ToLower())
						   select file).ToList();

			if (string.IsNullOrEmpty(searchWord))
			{
				if (!string.IsNullOrEmpty(exceptWord))
					return subFile.Where(m => !m.ToLower().Contains(exceptWord.ToLower())).ToList();
				else return subFile;
			}
			else
			{
				return subFile.Where(m => m.ToLower().Contains(searchWord.ToLower())).ToList();
			}
		}

		/// <summary>
		/// 通过对传入的文本来进行是否分段判断
		/// </summary>
		/// <param name="content">输入内容</param>
		/// <param name="splitStr">分割字符串</param>
		/// <returns>True:分段（多是手动分段）,False:不分段（系统默认）</returns>
		public static bool IsNewline(string content, string splitStr = "\n")
		{
			if (string.IsNullOrEmpty(content)) return false;

			content = content.TrimStart().TrimEnd();
			//隔行
			var list = Regex.Split(content, splitStr);
			var resultList = new List<string>();
			list.ToList().ForEach(u => resultList.Add(u.Trim()));
			var count = resultList.Count(string.IsNullOrEmpty);

			if (count > 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 移除内容中的空白行
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static string RemoveEmptyLines(string content)
		{
			if (string.IsNullOrEmpty(content)) return content;

			StringReader reader = new StringReader(content);
			StringBuilder sBuilder = new StringBuilder();

			var currentLine = reader.ReadLine();

			while (currentLine != null)
			{
				if (!string.IsNullOrEmpty(currentLine)) sBuilder.AppendLine(currentLine);
				currentLine = reader.ReadLine();
			}
			return sBuilder.ToString();
		}


		/// <summary>
		/// 备份文件以及文件夹
		/// </summary>
		/// <param name="sourcePath"></param>
		/// <param name="destinationPath"></param>
		public static void BackFolderFile(string sourcePath, string destinationPath)
		{
			try
			{
				if (!Directory.Exists(destinationPath))
				{
					Directory.CreateDirectory(destinationPath);
				}
				foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
				{
					Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
				}
				foreach (string newPath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
				{
					File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// 递归删除某个文件夹下的文件和文件夹
		/// </summary>
		/// <param name="path">需要删除的文件夹名称路径</param>
		public static void RecusiveDelete(string path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		/// <summary>
		/// 写日志，固定格式如："1@2@3@4",默认以追加的方式添加文件内容
		/// 第二项内容统一放路径
		/// </summary>
		/// <param name="path"></param>
		/// <param name="logList"></param>
		/// <param name="isCheck">检查第二项内容（需保持唯一）是否已存在与日志中</param>
		/// <param name="fileName">文件名称，若没有则以当前系统日期作为文件名称</param>
		/// <param name="append">是否是以追加的方式记录日志</param>
		public static void WriteLog(string path, List<string> logList, bool isCheck = false, string fileName = "",
			bool append = true)
		{
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			fileName = string.IsNullOrEmpty(fileName) ? DateTime.Now.ToString("yyyy-MM") + ".txt" : fileName;
			var logpath = Path.Combine(path, fileName);

			//是否屏蔽已有记录的日志
			if (isCheck)
			{
				FileStream fileStream = null;
				if (!File.Exists(logpath)) fileStream = File.Create(logpath);
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}

				//先读取文件检查是否以存在相关日志数据
				List<string> readList = new List<string>();
				using (StreamReader reader = new StreamReader(logpath, System.Text.Encoding.Default))
				{
					var line = string.Empty;
					while ((line = reader.ReadLine()) != null)
					{
						readList.Add(line.Split('@')[1]);
					}
				}

				using (StreamWriter writer = new StreamWriter(logpath, append, System.Text.Encoding.UTF8))
				{
					foreach (var log in logList)
					{
						var tempPath = log.Split('@')[1];
						if (!readList.Contains(tempPath))
						{
							writer.WriteLine(log);
						}
					}
				}
			}
			else
			{
				using (StreamWriter writer = new StreamWriter(logpath, append, System.Text.Encoding.UTF8))
				{
					foreach (var log in logList)
					{
						writer.WriteLine(log);
					}
				}
			}
		}

		/// <summary>
		/// 写日志，单行日志
		/// </summary>
		/// <param name="path"></param>
		/// <param name="fileName"></param>
		/// <param name="line"></param>
		public static void WriteLog(string path, string fileName, string line)
		{
			lock (_lock)
			{
				if (!Directory.Exists(path)) Directory.CreateDirectory(path);
				var filePath = Path.Combine(path, fileName);
				FileStream stream = null;
				if (!File.Exists(filePath)) stream = File.Create(filePath);
				if (stream != null)
				{
					stream.Close();
					stream.Dispose();
				}

				using (StreamWriter writer = new StreamWriter(filePath, true, System.Text.Encoding.UTF8))
				{
					writer.WriteLine($"{line}");
					writer.Close();
				}
			}
		}

		/// <summary>
		/// 检测文件是否被其他进程占用
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool CheckFileIsBeingUsed(string filePath, ref int count)
		{
			count++;
			FileInfo fileInfo = new FileInfo(filePath);
			FileStream stream = null;

			try
			{
				stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException)
			{
				return true;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}

			return false;
		}


		/// <summary>
		/// 根据指定搜索模式搜索目录列表
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="searchPattern"></param>
		/// <returns></returns>
		public static List<DirectoryInfo> GetSpecifiedFolderList(string folderPath, string searchPattern = "*-*-*-*-*")
		{
			try
			{
				if (!Directory.Exists(folderPath)) return new List<DirectoryInfo>();
				DirectoryInfo dir = new DirectoryInfo(folderPath);
				DirectoryInfo[] dirArray = dir.GetDirectories(searchPattern);
				List<DirectoryInfo> list = dirArray.ToList();
				return list;
			}
			catch (Exception e)
			{
				throw new Exception("查找指定类型目录失败,异常信息:" + e);
			}
		}

		/// <summary>
		/// 查看指定路径下文件数量
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public static List<FileInfo> GetSpecifiedFileList(string folderPath)
		{
			try
			{
				if (!Directory.Exists(folderPath)) return new List<FileInfo>();
				DirectoryInfo dir = new DirectoryInfo(folderPath);
				return dir.GetFiles().ToList();
			}
			catch (Exception e)
			{
				throw new Exception("查找指定类型目录失败,异常信息:" + e);
			}
		}

		/// <summary>
		/// 计算文件夹大小
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static long GetFolderSize(string path)
		{
			string[] allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			long sum = 0;
			foreach (var file in allFiles)
			{
				FileInfo info = new FileInfo(file);
				sum += info.Length;
			}
			return sum;
		}


		///// <summary>
		///// 辅助方法，检测日志中IP出现次数
		///// </summary>
		///// <param name="filePath"></param>
		///// <returns></returns>
		//public static List<string> AnalyzeIpTimes(string filePath)
		//{
		//	if (string.IsNullOrEmpty(filePath)) return new List<string>();
		//	if (!File.Exists(filePath)) return new List<string>();

		//	var ipAddressList = new List<string>();
		//	var result = new List<string>();
		//	var regex = new Regex(RegularExpression.IPAddressRegexExpression);
		//	var logLines = File.ReadAllLines(filePath, Encoding.UTF8);
		//	foreach (var logLine in logLines)
		//	{
		//		if (regex.IsMatch(logLine))
		//		{
		//			ipAddressList.Add(regex.Match(logLine).Value);
		//		}
		//	}

		//	var distinctIpAddress = ipAddressList.Distinct();

		//	foreach (var ipaddress in distinctIpAddress)
		//	{
		//		var count = ipAddressList.Count(m => m == ipaddress);
		//		result.Add(ipaddress + "@" + count);
		//	}

		//	return result;
		//}

		/// <summary>
		/// 重命名文件名称
		/// </summary>
		/// <param name="originFileName"></param>
		/// <param name="reNameFileName"></param>
		public static void RenameSpecifiedFileName(string originFileName, string reNameFileName)
		{
			var originName = Path.GetFileName(originFileName);
			var reNameFilePath = originFileName.Replace(originName, reNameFileName);
			File.Move(originFileName, reNameFilePath);
		}


		/// <summary>
		/// 获取Html文件中的去除标签的纯文本信息
		/// </summary>
		/// <param name="htmlContent"></param>
		/// <returns></returns>
		public static string GetIndexHtmlNoTagContent(string htmlContent)
		{
			if (string.IsNullOrEmpty(htmlContent)) return "";

			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(htmlContent);

			StringBuilder sBuilder = new StringBuilder();
			var nodeCollection = htmlDoc.DocumentNode.SelectNodes(@"//p[@class='paragraph']");
			if (nodeCollection != null)
			{
				for (var index = 0; index < nodeCollection.Count; index++)
				{
					var text = nodeCollection[index].InnerText.Trim();
					sBuilder.Append(text);
				}
			}
			return sBuilder.ToString();
		}


		/// <summary>
		/// 使用GB2312带检测BOM方式读取文件
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ReadFileWithGB2312AndDetectBOM(string path)
		{
			using (var reader = new StreamReader(path, Encoding.GetEncoding("GB2312"), true))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// 遍历目录下的所有指定格式文件
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="searchPattern"></param>
		/// <param name="files"></param>
		public static void GetAllFilesFromDirectory(string directory, string searchPattern, ref List<string> files)
		{
			var directories = Directory.GetDirectories(directory);
			files.AddRange(Directory.GetFiles(directory, searchPattern));

			foreach (var direcotry in directories)
			{
				GetAllFilesFromDirectory(direcotry, searchPattern, ref files);
			}
		}


		/// <summary>
		/// 获取当前程序集执行路径
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentExcutePath()
		{
			string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
			UriBuilder uri = new UriBuilder(codeBase);
			string path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}

		/// <summary>
		/// 读取大文件
		/// </summary>
		/// <param name="filePath"></param>
		public static void ReadBigFileLineByLine(string filePath)
		{
			using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (BufferedStream bs = new BufferedStream(fs))
			using (StreamReader sr = new StreamReader(bs))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					Console.WriteLine(line);
				}
			}
		}

		public static void DecompressGzFile(FileInfo fileToDecompress)
		{
			using (FileStream originalFileStream = fileToDecompress.OpenRead())
			{
				string currentFileName = fileToDecompress.FullName;
				string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

				using (FileStream decompressedFileStream = File.Create(newFileName))
				{
					using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
					{
						decompressionStream.CopyTo(decompressedFileStream);
					}
				}
			}
		}

		/// <summary>
		/// XML反序列化
		/// </summary>
		/// <param name="type">目标类型(Type类型)</param>
		/// <param name="filePath">XML文件路径</param>
		/// <returns>序列对象</returns>
		public static object DeserializeFromXML(Type type, string filePath)
		{
			if (!filePath.Exists() || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
			{
				return null;
			}
			FileStream fs = null;
			try
			{
				fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				XmlSerializer serializer = new XmlSerializer(type);
				return serializer.Deserialize(fs);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (fs != null)
					fs.Close();
			}
		}


		#region 扩展string操作文件方法

		/// <summary>
		/// 获取文件的文件夹路径
		/// </summary>
		/// <param name="path">路径</param>
		/// <returns></returns>
		public static string GetDirectoryName(this string path)
		{
			var fileName = Path.GetDirectoryName(path) ?? "";
			return fileName;
		}

		/// <summary>
		/// 获取文件名
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="isContainsExtension">是否包含扩展名 默认包含</param>
		/// <returns></returns>
		public static string GetFileName(this string path, bool isContainsExtension = true)
		{
			var fileName = Path.GetFileName(path);
			if (isContainsExtension || fileName == null)
			{
				return fileName;
			}
			var dotIndex = fileName.LastIndexOf('.');
			if (dotIndex <= -1)
			{
				//无扩展名
				return fileName;
			}
			return fileName.Substring(0, dotIndex);
		}

		/// <summary>
		/// 获取扩展名
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="isContainsDot">扩展名中是否包含.</param>
		/// <returns></returns>
		public static string GetExtension(this string path, bool isContainsDot = true)
		{
			var extension = Path.GetExtension(path);
			if (isContainsDot || extension == null || extension.Length < 1)
			{
				return extension;
			}
			return extension.Substring(1);
		}

		/// <summary>
		/// 判断文件是否存在
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="isCreateEmptyFileWhenFileIsNotExists">当文件不存在的时候 是否创建空文件 默认不创建</param>
		/// <returns></returns>
		public static bool Exists(this string filePath, bool isCreateEmptyFileWhenFileIsNotExists = false)
		{
			var exists = System.IO.File.Exists(filePath);
			if (exists)
			{
				return true;
			}
			if (!isCreateEmptyFileWhenFileIsNotExists)
			{
				return false;
			}
			System.IO.File.Create(filePath).Dispose();
			return true;
		}

		/// <summary>
		/// 把文件复制到目标路径中
		/// </summary>
		/// <param name="sourceFilePath">源文件</param>
		/// <param name="targetFilePath">目标文件</param>
		/// <param name="isOverwrite">是否重写 默认不重写</param>
		public static void ToCopy(this string sourceFilePath, string targetFilePath, bool isOverwrite = false)
		{
			if (!sourceFilePath.Exists())
			{
				return;
			}
			System.IO.File.Copy(sourceFilePath, targetFilePath, isOverwrite);
		}

		/// <summary>
		/// 路径添加
		/// </summary>
		/// <param name="basePath">基路径</param>
		/// <param name="paths">后续路径</param>
		/// <returns></returns>
		public static string Combine(this string basePath, params string[] paths)
		{
			if (paths == null)
			{
				return basePath;
			}
			var pathList = paths.ToList();
			pathList.Insert(0, basePath);
			return System.IO.Path.Combine(pathList.ToArray());
		}

		/// <summary>
		/// 判断文件夹是否存在
		/// </summary>
		/// <param name="dir">文件夹路径</param>
		/// <param name="isCreateDirWhenDirIsNotExists">当文件夹不存在的时候 是否创建文件夹 默认不创建</param>
		/// <returns></returns>
		public static bool ExistsDirectory(this string dir, bool isCreateDirWhenDirIsNotExists = false)
		{
			var exists = System.IO.Directory.Exists(dir);
			if (exists)
			{
				return true;
			}
			if (!isCreateDirWhenDirIsNotExists)
			{
				return false;
			}
			System.IO.Directory.CreateDirectory(dir);
			return true;
		}

		#endregion

		#region 判断文件的编码格式
		/// <summary>
		/// 判断文件的编码格式
		/// </summary>
		/// <param name="filename">文件路径</param>
		/// <returns></returns>
		public static System.Text.Encoding GetFileEncodeType(this string filename)
		{
			using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.BinaryReader br = new System.IO.BinaryReader(fs))
				{
					Byte[] buffer = br.ReadBytes(2);
					if (buffer[0] >= 0xEF)
					{
						if (buffer[0] == 0xEF && buffer[1] == 0xBB)
						{
							return System.Text.Encoding.UTF8;
						}
						if (buffer[0] == 0xFE && buffer[1] == 0xFF)
						{
							return System.Text.Encoding.BigEndianUnicode;
						}
						if (buffer[0] == 0xFF && buffer[1] == 0xFE)
						{
							return System.Text.Encoding.Unicode;
						}
						return System.Text.Encoding.Default;
					}
					return System.Text.Encoding.Default;
				}
			}
		}
		#endregion

		#region 校验文件编码格式
		/// <summary>
		/// 检查文件编码格式,是否是utf-8 如果不是utf-8则重写文件使得是utf-8
		/// </summary>
		/// <param name="path">文件路径</param>
		public static void CheckFileEncodeIsUtf8IfNotUtf8RewriteTheFile(this string path)
		{
			path.CheckFileEncodeIfNotRewriteTheFile(Encoding.UTF8);
		}

		/// <summary>
		/// 检查文件编码格式,是否是指定的编码格式 如果不是指定的编码格式, 则重写文件使得是指定的编码格式
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <param name="encoding">要指定的编码格式</param>
		public static void CheckFileEncodeIfNotRewriteTheFile(this string path, Encoding encoding)
		{
			try
			{
				if (!path.Exists())
				{
					return;
				}
				var oldEncoding = path.GetFileEncodeType();
				if (oldEncoding == encoding)
				{
					return;
				}
				var content = File.ReadAllText(path, oldEncoding);
				File.WriteAllText(path, content, encoding);
			}
			catch (Exception ex)
			{
				//do nothing
			}
		}
		#endregion
	}
}
