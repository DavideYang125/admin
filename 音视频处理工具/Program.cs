using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("*************音视频工具********************");
            Console.WriteLine("【1】根据时间切割视频");
          
            Console.WriteLine("*************小工具********************");
            Console.Write("请输入操作类型前的数字:");
            var num = Console.ReadLine();
            Console.WriteLine("您输入的是:" + num);
            switch(num)
            {
                case "1":
                    ToolKit.MediaHelper.CarveVideo();
                    break;
                default:
                    return;
            }
            Console.WriteLine("处理完成，按任意键退出");
            Console.ReadKey();
        }
    }
}
