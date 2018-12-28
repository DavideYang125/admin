/// <summary>
/// ��˵����Assistant
/// �� �� �ˣ��շ�
/// ��ϵ��ʽ��361983679  
/// ������վ��http://www.sufeinet.com/thread-655-1-1.html
/// </summary>
using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace DotNet.Utilities
{
    /// <summary>
    /// �ļ��ϴ���
    /// </summary>
    public class FileUp
    {
        public FileUp()
        { }

        /// <summary>
        /// ת��Ϊ�ֽ�����
        /// </summary>
        /// <param name="filename">�ļ���</param>
        /// <returns>�ֽ�����</returns>
        public byte[] GetBinaryFile(string filename)
        {
            if (File.Exists(filename))
            {
                FileStream Fsm = null;
                try
                {
                    Fsm = File.OpenRead(filename);
                    return this.ConvertStreamToByteBuffer(Fsm);
                }
                catch
                {
                    return new byte[0];
                }
                finally
                {
                    Fsm.Close();
                }
            }
            else
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// ��ת��Ϊ�ֽ�����
        /// </summary>
        /// <param name="theStream">��</param>
        /// <returns>�ֽ�����</returns>
        public byte[] ConvertStreamToByteBuffer(System.IO.Stream theStream)
        {
            int bi;
            MemoryStream tempStream = new System.IO.MemoryStream();
            try
            {
                while ((bi = theStream.ReadByte()) != -1)
                {
                    tempStream.WriteByte(((byte)bi));
                }
                return tempStream.ToArray();
            }
            catch
            {
                return new byte[0];
            }
            finally
            {
                tempStream.Close();
            }
        }

        /// <summary>
        /// �ϴ��ļ�
        /// </summary>
        /// <param name="PosPhotoUpload">�ؼ�</param>
        /// <param name="saveFileName">������ļ���</param>
        /// <param name="imagePath">������ļ�·��</param>
        public string FileSc(FileUpload PosPhotoUpload, string saveFileName, string imagePath)
        {
            string state = "";
            if (PosPhotoUpload.HasFile)
            {
                if (PosPhotoUpload.PostedFile.ContentLength / 1024 < 10240)
                {
                    string MimeType = PosPhotoUpload.PostedFile.ContentType;
                    if (String.Equals(MimeType, "image/gif") || String.Equals(MimeType, "image/pjpeg"))
                    {
                        string extFileString = System.IO.Path.GetExtension(PosPhotoUpload.PostedFile.FileName);
                        PosPhotoUpload.PostedFile.SaveAs(HttpContext.Current.Server.MapPath(imagePath));
                    }
                    else
                    {
                        state = "�ϴ��ļ����Ͳ���ȷ";
                    }
                }
                else
                {
                    state = "�ϴ��ļ����ܴ���10M";
                }
            }
            else
            {
                state = "û���ϴ��ļ�";
            }
            return state;
        }

        /// <summary>
        /// �ϴ��ļ�
        /// </summary>
        /// <param name="binData">�ֽ�����</param>
        /// <param name="fileName">�ļ���</param>
        /// <param name="fileType">�ļ�����</param>
        //-------------------����----------------------
        //byte[] by = GetBinaryFile("E:\\Hello.txt");
        //this.SaveFile(by,"Hello",".txt");
        //---------------------------------------------
        public void SaveFile(byte[] binData, string fileName, string fileType)
        {
            FileStream fileStream = null;
            MemoryStream m = new MemoryStream(binData);
            try
            {
                string savePath = HttpContext.Current.Server.MapPath("~/File/");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                string File = savePath + fileName + fileType;
                fileStream = new FileStream(File, FileMode.Create);
                m.WriteTo(fileStream);
            }
            finally
            {
                m.Close();
                fileStream.Close();
            }
        }
    }
}