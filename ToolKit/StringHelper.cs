using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToolKit
{
    public class StringHelper
    {
        #region simple and traditional 
        /// <summary>
        /// to simple
        /// </summary>
        public static string TraditionalToSimplifiedChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.SimplifiedChinese, 0);
        }

        /// <summary>
        /// to traditional
        /// </summary>
        public static string SimplifiedToTraditionalChinese(string s)
        {
            return Strings.StrConv(s, VbStrConv.TraditionalChinese, 0);
        }

        #endregion
    }
}
