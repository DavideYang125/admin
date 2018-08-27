using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToolKit
{
    public class LrcHelper
    {

        /// <summary>
        /// 所有语言字符
        /// </summary>
        private static Regex _characterRegex = new Regex("[a-zA-ZÀÈÌÒÙàèìòùÁÉÍÓÚÝáéíóúýÂÊÎÔÛâêîôûÃÑÕãñõÄËÏÖÜŸäëïöüŸ¡¿çÇŒœßØøÅåÆæÞþÐð\\d]");

        /// <summary>
        /// 时间戳
        /// </summary>
        private static Regex _timeRegex = new Regex(@"\[[\d-]{2}:[\d-]{2}[:\.][\d-]{2}\.{0,1}[\d-]{0,3}\]");

        /// <summary>
        /// 获取字幕不含时间戳 即移除时间戳
        /// </summary>
        /// <param name="lrcContent">字幕</param>
        /// <returns></returns>
        public static string GetLrcContentWithoutTimeSpan(string lrcContent)
        {
            var newLrcContent = _timeRegex.Replace(lrcContent, "");
            return newLrcContent;
        }

        /// <summary>
        /// 把srt转为lrc 
        /// </summary>
        /// <param name="lrcContent"></param>
        /// <param name="withEndTimestamp">是否包含结束时间</param>
        /// <param name="withSubtitleType">是否包含字幕类型</param>
        /// <returns></returns>
        public static string ConvertSrt2Lrc(string lrcContent, bool withEndTimestamp = false, bool withSubtitleType = false)
        {
            lrcContent = lrcContent.Replace("\r\n", "\n").Replace("\n", "\r\n");
            StringBuilder sBuilder = new StringBuilder();
            var srtLines = lrcContent.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            if (srtLines.Count() <= 1)
            {
                srtLines = lrcContent.Split('\n').ToList();
            }
            var timeLine = string.Empty;
            var contentLine = string.Empty;
            var transLine = string.Empty;

            for (int index = 0; index < srtLines.Count(); index++)
            {
                var currentLine = srtLines[index];
                if (index + 1 < srtLines.Count())
                {
                    var nextLine = srtLines[index + 1];
                    if (Regex.IsMatch(currentLine.Trim(), @"^[0-9A-Za-z]+$") && nextLine.Contains("-->"))
                    {
                        if (!string.IsNullOrEmpty(timeLine) && (!string.IsNullOrEmpty(contentLine) || !string.IsNullOrEmpty(transLine)))
                        {
                            //针对仅有译文的srt做处理，例如从TED采集的字幕文件
                            if (string.IsNullOrEmpty(contentLine)) contentLine = "###videoconvertline###";

                            sBuilder.AppendLine(timeLine + contentLine);

                            if (!string.IsNullOrEmpty(transLine))
                            {
                                if (!_characterRegex.IsMatch(transLine))
                                {
                                    transLine = ToSimplifiedChinese(transLine);
                                }
                                sBuilder.AppendLine(transLine.Trim());
                            }
                            sBuilder.AppendLine("");

                            timeLine = string.Empty;
                            contentLine = string.Empty;
                            transLine = string.Empty;
                        }
                        if (Regex.IsMatch(currentLine.Trim(), @"^[A-Za-z]$"))
                        {
                            if (withSubtitleType)
                            {
                                sBuilder.Append($"[{currentLine}]");
                            }
                        }
                        continue;
                    }
                }

                if (currentLine.Contains("-->"))
                {
                    //默认使用第一个时间戳
                    if (string.IsNullOrEmpty(timeLine))
                    {
                        var startTime = Regex.Split(currentLine, "-->")[0];
                        var endTime = Regex.Split(currentLine, "-->")[1];

                        if (startTime.Contains(","))
                        {
                            startTime = startTime.Replace(",", ".").Trim();
                            string tempStartTime;
                            if (startTime.Length > 11)
                            {
                                tempStartTime = startTime.Substring(3, 9);
                            }
                            else if (startTime.Length > 10)
                            {
                                tempStartTime = startTime.Substring(3, 8);
                            }
                            else
                            {
                                tempStartTime = startTime.Substring(3, 7) + "5";
                            }
                            timeLine = "[" + tempStartTime + "]";
                        }
                        else
                        {
                            timeLine = "[" + startTime.Substring(0, startTime.Length - 1) + "]";
                        }
                        if (withEndTimestamp)
                        {
                            if (endTime.Contains(","))
                            {
                                endTime = endTime.Replace(",", ".").Trim();
                                string tempEndTime;
                                if (endTime.Length > 11)
                                {
                                    tempEndTime = endTime.Substring(3, 9);
                                }
                                else if (endTime.Length > 10)
                                {
                                    tempEndTime = endTime.Substring(3, 8);
                                }
                                else
                                {
                                    tempEndTime = endTime.Substring(3, 7) + "5";
                                }
                                timeLine += "[" + tempEndTime + "]";
                            }
                            else
                            {
                                timeLine += "[" + endTime.Substring(1, endTime.Length - 1) + "]";
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(currentLine) && string.IsNullOrEmpty(contentLine) && string.IsNullOrEmpty(transLine)) continue;

                    if (!HasChinese(currentLine))
                    {
                        contentLine += " " + currentLine;
                    }
                    else
                    {
                        transLine += " " + currentLine;
                    }
                }

                if (index + 1 == srtLines.Count())
                {
                    if (!string.IsNullOrEmpty(timeLine) && (!string.IsNullOrEmpty(contentLine) || !string.IsNullOrEmpty(transLine)))
                    {
                        //针对仅有译文的srt做处理，例如从TED采集的字幕文件
                        if (string.IsNullOrEmpty(contentLine.Trim())) contentLine = "###videoconvertline###";

                        sBuilder.AppendLine(timeLine + contentLine);

                        if (!string.IsNullOrEmpty(transLine))
                        {
                            if (!_characterRegex.IsMatch(transLine))
                            {
                                transLine = ToSimplifiedChinese(transLine);
                            }
                            sBuilder.AppendLine(transLine.Trim());
                        }
                        sBuilder.AppendLine("");

                        timeLine = string.Empty;
                        contentLine = string.Empty;
                        transLine = string.Empty;
                    }
                    else if (withSubtitleType)
                    {
                        sBuilder.Append($"[{currentLine}]");
                    }
                }
            }

            var ss = sBuilder.ToString();
            return sBuilder.ToString();
        }

        /// <summary>
        /// 如果里面包含一个汉字，就会返回true.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasChinese(string text)
        {
            bool flag = true;
            if (!string.IsNullOrEmpty(text))
            {
                text = ReplaceStringText(text);
                if (string.IsNullOrEmpty(text)) return false;

                char[] c = text.ToCharArray();

                foreach (char t in c)
                {
                    if (!char.IsNumber(t))
                    {
                        if (t < 0x4e00 || t > 0x9fbb)
                        {
                            //Console.WriteLine("不是汉字");
                            flag = false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        private static string ReplaceStringText(string text)
        {
            text =
                text.Replace(",", "")
                    .Replace("，", "")
                    .Replace(".", "")
                    .Replace("。", "")
                    .Replace("!", "")
                    .Replace(";", "")
                    .Replace("'", "")
                    .Replace("\"", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("《", "")
                    .Replace("《", "")
                    .Trim()
                    .TrimStart()
                    .TrimEnd()
                    .Replace(" ", "");
            return text;
        }

        #region 简繁
        /// <summary>
        /// 转换为简体中文
        /// </summary>
        public static string ToSimplifiedChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.SimplifiedChinese, 0);
        }

        /// <summary>
        /// 转换为繁体中文
        /// </summary>
        public static string ToTraditionalChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.TraditionalChinese, 0);
        }

        #endregion
    }
}
