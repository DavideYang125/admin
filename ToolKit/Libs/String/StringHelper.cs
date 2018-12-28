 /// <summary>
/// ��˵����Assistant
/// �� �� �ˣ��շ�
/// ��ϵ��ʽ��361983679  
/// ������վ��http://www.sufeinet.com/thread-655-1-1.html
/// </summary>
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Utilities
{
    /// <summary>
    /// �ַ���������
    /// 1��GetStrArray(string str, char speater, bool toLower)  ���ַ������շָ���ת���� List
    /// 2��GetStrArray(string str) ���ַ���ת ����, �ָ� ��Ϊ����
    /// 3��GetArrayStr(List list, string speater) �� List ���շָ�����װ�� string
    /// 4��GetArrayStr(List list)  �õ������б��Զ��ŷָ����ַ���
    /// 5��GetArrayValueStr(Dictionary<int, int> list)�õ������б��Զ��ŷָ����ַ���
    /// 6��DelLastComma(string str)ɾ������β��һ������
    /// 7��DelLastChar(string str, string strchar)ɾ������β��ָ���ַ�����ַ�
    /// 8��ToSBC(string input)תȫ�ǵĺ���(SBC case)
    /// 9��ToDBC(string input)ת��ǵĺ���(SBC case)
    /// 10��GetSubStringList(string o_str, char sepeater)���ַ�������ָ���ָ���װ�� List ȥ���ظ�
    /// 11��GetCleanStyle(string StrList, string SplitString)���ַ�����ʽת��Ϊ���ַ���
    /// 12��GetNewStyle(string StrList, string NewStyle, string SplitString, out string Error)���ַ���ת��Ϊ����ʽ
    /// 13��SplitMulti(string str, string splitstr)�ָ��ַ���
    /// 14��SqlSafeString(string String, bool IsDel)
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// ���ַ������շָ���ת���� List
        /// </summary>
        /// <param name="str">Դ�ַ���</param>
        /// <param name="speater">�ָ���</param>
        /// <param name="toLower">�Ƿ�ת��ΪСд</param>
        /// <returns></returns>
        public static List<string> GetStrArray(string str, char speater, bool toLower)
        {
            List<string> list = new List<string>();
            string[] ss = str.Split(speater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != speater.ToString())
                {
                    string strVal = s;
                    if (toLower)
                    {
                        strVal = s.ToLower();
                    }
                    list.Add(strVal);
                }
            }
            return list;
        }
        /// <summary>
        /// ���ַ���ת ����, �ָ� ��Ϊ����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] GetStrArray(string str)
        {
            return str.Split(new Char[] { ',' });
        }
        /// <summary>
        /// �� List<string> ���շָ�����װ�� string
        /// </summary>
        /// <param name="list"></param>
        /// <param name="speater"></param>
        /// <returns></returns>
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// �õ������б��Զ��ŷָ����ַ���
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetArrayStr(List<int> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i].ToString());
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// �õ������б��Զ��ŷָ����ַ���
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetArrayValueStr(Dictionary<int, int> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, int> kvp in list)
            {
                sb.Append(kvp.Value + ",");
            }
            if (list.Count > 0)
            {
                return DelLastComma(sb.ToString());
            }
            else
            {
                return "";
            }
        }

        #region ɾ�����һ���ַ�֮����ַ�

        /// <summary>
        /// ɾ������β��һ������
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }

        /// <summary>
        /// ɾ������β��ָ���ַ�����ַ�
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar));
        }

        #endregion

        /// <summary>
        /// תȫ�ǵĺ���(SBC case)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            //���תȫ�ǣ�
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>
        ///  ת��ǵĺ���(SBC case)
        /// </summary>
        /// <param name="input">����</param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// ���ַ�������ָ���ָ���װ�� List ȥ���ظ�
        /// </summary>
        /// <param name="o_str"></param>
        /// <param name="sepeater"></param>
        /// <returns></returns>
        public static List<string> GetSubStringList(string o_str, char sepeater)
        {
            List<string> list = new List<string>();
            string[] ss = o_str.Split(sepeater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != sepeater.ToString())
                {
                    list.Add(s);
                }
            }
            return list;
        }


        #region ���ַ�����ʽת��Ϊ���ַ���
        /// <summary>
        ///  ���ַ�����ʽת��Ϊ���ַ���
        /// </summary>
        /// <param name="StrList"></param>
        /// <param name="SplitString"></param>
        /// <returns></returns>
        public static string GetCleanStyle(string StrList, string SplitString)
        {
            string RetrunValue = "";
            //���Ϊ�գ����ؿ�ֵ
            if (StrList == null)
            {
                RetrunValue = "";
            }
            else
            {
                //����ȥ���ָ���
                string NewString = "";
                NewString = StrList.Replace(SplitString, "");
                RetrunValue = NewString;
            }
            return RetrunValue;
        }
        #endregion

        #region ���ַ���ת��Ϊ����ʽ
        /// <summary>
        /// ���ַ���ת��Ϊ����ʽ
        /// </summary>
        /// <param name="StrList"></param>
        /// <param name="NewStyle"></param>
        /// <param name="SplitString"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public static string GetNewStyle(string StrList, string NewStyle, string SplitString, out string Error)
        {
            string ReturnValue = "";
            //��������ֵ�����ؿգ�������������ʾ
            if (StrList == null)
            {
                ReturnValue = "";
                Error = "��������Ҫ���ָ�ʽ���ַ���";
            }
            else
            {
                //��鴫����ַ������Ⱥ���ʽ�Ƿ�ƥ��,�����ƥ�䣬��˵��ʹ�ô��󡣸���������Ϣ�����ؿ�ֵ
                int strListLength = StrList.Length;
                int NewStyleLength = GetCleanStyle(NewStyle, SplitString).Length;
                if (strListLength != NewStyleLength)
                {
                    ReturnValue = "";
                    Error = "��ʽ��ʽ�ĳ�����������ַ����Ȳ���������������";
                }
                else
                {
                    //�������ʽ�зָ�����λ��
                    string Lengstr = "";
                    for (int i = 0; i < NewStyle.Length; i++)
                    {
                        if (NewStyle.Substring(i, 1) == SplitString)
                        {
                            Lengstr = Lengstr + "," + i;
                        }
                    }
                    if (Lengstr != "")
                    {
                        Lengstr = Lengstr.Substring(1);
                    }
                    //���ָ�����������ʽ�е�λ��
                    string[] str = Lengstr.Split(',');
                    foreach (string bb in str)
                    {
                        StrList = StrList.Insert(int.Parse(bb), SplitString);
                    }
                    //�������Ľ��
                    ReturnValue = StrList;
                    //��Ϊ�������������û�д���
                    Error = "";
                }
            }
            return ReturnValue;
        }
        #endregion

        /// <summary>
        /// �ָ��ַ���
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitstr"></param>
        /// <returns></returns>
        public static string[] SplitMulti(string str, string splitstr)
        {
            string[] strArray = null;
            if ((str != null) && (str != ""))
            {
                strArray = new Regex(splitstr).Split(str);
            }
            return strArray;
        }
        public static string SqlSafeString(string String, bool IsDel)
        {
            if (IsDel)
            {
                String = String.Replace("'", "");
                String = String.Replace("\"", "");
                return String;
            }
            String = String.Replace("'", "'");
            //String = String.Replace("\"", """);
            return String;
        }

        #region ��ȡ��ȷ��Id���������������������0
        /// <summary>
        /// ��ȡ��ȷ��Id���������������������0
        /// </summary>
        /// <param name="_value"></param>
        /// <returns>������ȷ������ID��ʧ�ܷ���0</returns>
        public static int StrToId(string _value)
        {
            if (IsNumberId(_value))
                return int.Parse(_value);
            else
                return 0;
        }
        #endregion
        #region ���һ���ַ����Ƿ��Ǵ����ֹ��ɵģ�һ�����ڲ�ѯ�ַ�����������Ч����֤��
        /// <summary>
        /// ���һ���ַ����Ƿ��Ǵ����ֹ��ɵģ�һ�����ڲ�ѯ�ַ�����������Ч����֤��(0����)
        /// </summary>
        /// <param name="_value">����֤���ַ�������</param>
        /// <returns>�Ƿ�Ϸ���boolֵ��</returns>
        public static bool IsNumberId(string _value)
        {
            return QuickValidate("^[1-9]*[0-9]*$", _value);
        }
        #endregion
        #region ������֤һ���ַ����Ƿ����ָ�����������ʽ��
        /// <summary>
        /// ������֤һ���ַ����Ƿ����ָ�����������ʽ��
        /// </summary>
        /// <param name="_express">�������ʽ�����ݡ�</param>
        /// <param name="_value">����֤���ַ�����</param>
        /// <returns>�Ƿ�Ϸ���boolֵ��</returns>
        public static bool QuickValidate(string _express, string _value)
        {
            if (_value == null) return false;
            Regex myRegex = new Regex(_express);
            if (_value.Length == 0)
            {
                return false;
            }
            return myRegex.IsMatch(_value);
        }
        #endregion


        #region �������ö�ָ���ַ������� MD5 ����
        /// <summary>
        /// �������ö�ָ���ַ������� MD5 ����
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetMD5(string s)
        {
            //md5����
            s = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5").ToString();

            return s.ToLower().Substring(8, 16);
        }
        #endregion

        #region �õ��ַ������ȣ�һ�����ֳ���Ϊ2
        /// <summary>
        /// �õ��ַ������ȣ�һ�����ֳ���Ϊ2
        /// </summary>
        /// <param name="inputString">�����ַ���</param>
        /// <returns></returns>
        public static int StrLength(string inputString)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }
        #endregion

        #region ��ȡָ�������ַ���
        /// <summary>
        /// ��ȡָ�������ַ���
        /// </summary>
        /// <param name="inputString">Ҫ�������ַ���</param>
        /// <param name="len">ָ������</param>
        /// <returns>���ش�������ַ���</returns>
        public static string ClipString(string inputString, int len)
        {
            bool isShowFix = false;
            if (len % 2 == 1)
            {
                isShowFix = true;
                len--;
            }
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }

            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (isShowFix && mybyte.Length > len)
                tempString += "��";
            return tempString;
        }
        #endregion



        #region HTMLת�г�TEXT
        /// <summary>
        /// HTMLת�г�TEXT
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string HtmlToTxt(string strHtml)
        {
            string[] aryReg ={
            @"<script[^>]*?>.*?</script>",
            @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
            @"([\r\n])[\s]+",
            @"&(quot|#34);",
            @"&(amp|#38);",
            @"&(lt|#60);",
            @"&(gt|#62);", 
            @"&(nbsp|#160);", 
            @"&(iexcl|#161);",
            @"&(cent|#162);",
            @"&(pound|#163);",
            @"&(copy|#169);",
            @"&#(\d+);",
            @"-->",
            @"<!--.*\n"
            };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }

            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");


            return strOutput;
        }
        #endregion

        #region �ж϶����Ƿ�Ϊ��
        /// <summary>
        /// �ж϶����Ƿ�Ϊ�գ�Ϊ�շ���true
        /// </summary>
        /// <typeparam name="T">Ҫ��֤�Ķ��������</typeparam>
        /// <param name="data">Ҫ��֤�Ķ���</param>        
        public static bool IsNullOrEmpty<T>(T data)
        {
            //���Ϊnull
            if (data == null)
            {
                return true;
            }

            //���Ϊ""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //���ΪDBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //��Ϊ��
            return false;
        }

        /// <summary>
        /// �ж϶����Ƿ�Ϊ�գ�Ϊ�շ���true
        /// </summary>
        /// <param name="data">Ҫ��֤�Ķ���</param>
        public static bool IsNullOrEmpty(object data)
        {
            //���Ϊnull
            if (data == null)
            {
                return true;
            }

            //���Ϊ""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //���ΪDBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //��Ϊ��
            return false;
        }
        #endregion
    }
}