using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Spider;

namespace Crawler
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				GagSpider.DownLoadFiles();
				Thread.Sleep(600000);
			}
		}
	}
}
