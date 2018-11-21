using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AForge;
using AForge.Video;

namespace ToolKit
{
    
    public class MediaHelper
    {
        public static string _ffmpegPath = @"D:\tool\ffmpeg.exe";

        /// <summary>
        /// 如:时间格式如：00:44:59.516或者00:30:00
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outPath"></param>
        /// <param name="startTime"></param>
        /// <param name="endtime"></param>
        public static void CarveVideo(string inputPath, string outPath, string startTime, string duration)
        {
            Process _process = new Process();
            _process.StartInfo.FileName = _ffmpegPath;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = true;
            _process.StartInfo.Arguments = $"-i \"{inputPath}\" -vcodec copy -acodec copy -ss {startTime} -t {duration} \"{outPath}\"";
            _process.Start();
            string strOutput = "";
            //strOutput = _process.StandardOutput.ReadToEnd();

            _process.WaitForExit();
            _process.Close();
            //Process.Start(_ffmpegPath, $"-i {inputPath} -vcodec copy -acodec copy -ss {startTime} -t {duration} {outPath}");
            if (File.Exists(outPath)) Console.WriteLine("切割完成");
            else Console.WriteLine("切割失败");
        }
        public static void CarveVideo(string infoStr = "")
        {
            if(string.IsNullOrEmpty(infoStr))
            {
                Console.WriteLine("请输入切割信息（如d:\\1.mp4@d:\\2.mp4@00:00:00.010@00:02:25.040）:");
                infoStr = Console.ReadLine();

            }
            Console.WriteLine("切割信息是: " + infoStr);
            Console.WriteLine("正在切割...");
            var info = infoStr.Split('@');
            CarveVideo(info[0], info[1], info[2], info[3]);
        }
        public static bool CutOneSecondForVideo(string path)
        {
            var startTime = "00:00:00";
            //duration格式：00:01:22.25
            var dir = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var exten = Path.GetExtension(path);
            var newPath = Path.Combine(dir, fileName + "!" + exten);
            var duration = GetAudioDuration(path);

            var infos = duration.Split('.');
            var timeInfos = infos[0].Split(':');
            var secondStr = timeInfos[2];
            var secondNum = Convert.ToInt32(secondStr);
            var newSecondNum = secondNum - 1;
            var newSecondNumStr = newSecondNum.ToString();
            if (newSecondNumStr.Length == 1) newSecondNumStr = "0" + newSecondNumStr;
            var newDuration = timeInfos[0] + ":" + timeInfos[1] + ":" + newSecondNumStr + "." + infos[1];
            CarveVideo(path, newPath, startTime, newDuration);
            Thread.Sleep(100);
            if (File.Exists(newPath)) File.Delete(path);
            return true;
        }
        /// <summary>
        /// 时长》
        /// </summary>
        /// <param name="audioPath"></param>
        /// <returns></returns>
        public static string GetAudioDuration(string audioPath)
        {
            string duration = string.Empty;
            try
            {
                //Duration\:\s*\d{2}\:\d{2}\:\d{2}\.\d{2}
                string message = ExcuteFFmpeg("-i " + "\""+audioPath+"\"");
                Regex regex = new Regex(@"Duration\:\s*\d{2}\:\d{2}\:\d{2}\.\d{2}");

                if (regex.IsMatch(message))
                {
                    duration = regex.Match(message).ToString().Replace("Duration: ", "");
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("获得指定音频时长错误，音频路径：{0}，异常信息：{1}。", audioPath, e.Message));
            }
            return duration;
        }
        /// <summary>
        /// 执行FFmpeg
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string ExcuteFFmpeg(string arguments)
        {
            return ExcuteShell(arguments, _ffmpegPath);
        }
        public static string ExcuteShell(string excuteCmd, string fileName)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            Console.WriteLine(fileName + " " + excuteCmd);
            string message = string.Empty;
            try
            {
                int count = 0;
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = excuteCmd;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = UTF8Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = UTF8Encoding.UTF8;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                process.BeginOutputReadLine();
                message = process.StandardError.ReadToEnd();
                //最迟不允许超过30秒，超过30秒自动结束
                while (!process.HasExited)
                {
                    if (count == 300)
                    {
                        process.Kill();
                        return "";
                    }
                    Thread.Sleep(100);
                    count++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("执行{0}过程中产生异常，异常信息：{1}", excuteCmd, e.Message));
            }
            finally
            {
                process.Close();
                process.Dispose();
            }
            return message;
        }
        public static bool SingleImgToVideo(string imgPath, string videoPath)
        {
            
            //using (VideoFileWriter writer = new VideoFileWriter())
            //{
            //    writer.Open(@"d:\myfile.avi", 640, 480, 25, VideoCodec.MPEG4);
            //    foreach (var file in Directory.GetFiles(@"d:\foo\bar", "*.jpg"))
            //    {
            //        writer.WriteVideoFrame(Bitmap.FromFile(file) as Bitmap);
            //    }
            //    writer.Close();
            //}

            if (!File.Exists(videoPath)) return false;
            return true;
        }
    }
}
