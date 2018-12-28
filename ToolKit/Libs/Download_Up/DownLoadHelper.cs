/// <summary>
/// ��˵����Assistant
/// �� �� �ˣ��շ�
/// ��ϵ��ʽ��361983679  
/// ������վ��http://www.sufeinet.com/thread-655-1-1.html
/// </summary>
using System;
using System.IO;
using System.Web;
using System.Threading;

namespace DotNet.Utilities
{
    /// <summary>
    /// �ļ����ذ�����
    /// </summary>
    public class DownLoadHelper
    {
        #region ResponseFile ���Ӳ���ļ����ṩ���� ֧�ִ��ļ����������ٶ����ơ���Դռ��С
        /// <summary>
        ///  ���Ӳ���ļ����ṩ���� ֧�ִ��ļ����������ٶ����ơ���Դռ��С
        /// </summary>
        /// <param name="_Request">Page.Request����</param>
        /// <param name="_Response">Page.Response����</param>
        /// <param name="_fileName">�����ļ���</param>
        /// <param name="_fullPath">���ļ�������·��</param>
        /// <param name="_speed">ÿ���������ص��ֽ���</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public static bool ResponseFile(HttpRequest _Request, HttpResponse _Response, string _fileName, string _fullPath, long _speed)
        {
            try
            {
                FileStream myFile = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                try
                {
                    _Response.AddHeader("Accept-Ranges", "bytes");
                    _Response.Buffer = false;
                    long fileLength = myFile.Length;
                    long startBytes = 0;

                    int pack = 10240; //10K bytes
                    //int sleep = 200;   //ÿ��5��   ��5*10K bytesÿ��
                    int sleep = (int)Math.Floor((double)(1000 * pack / _speed)) + 1;
                    if (_Request.Headers["Range"] != null)
                    {
                        _Response.StatusCode = 206;
                        string[] range = _Request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    _Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        _Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }
                    _Response.AddHeader("Connection", "Keep-Alive");
                    _Response.ContentType = "application/octet-stream";
                    _Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(_fileName, System.Text.Encoding.UTF8));

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Floor((double)((fileLength - startBytes) / pack)) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (_Response.IsClientConnected)
                        {
                            _Response.BinaryWrite(br.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}