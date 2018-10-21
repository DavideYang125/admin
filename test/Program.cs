using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToolKit;

namespace test
{
	public	class Program
	{
		public static void Main(string[] args)
		{
            MediaHelper.CutOneSecondForVideo(@"F:\Project\video\eu\testvideo\1.mp4");
            var t = EncryptionHepler.DecodeBase64("");
          
            //ToolKit.Mail.SendQQMail("d", "test","testmail");
        }
    }
}
