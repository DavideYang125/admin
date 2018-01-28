using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
	public class BaseSpider
	{

	}

	public class SpiderModel
	{
		public Guid Id { get; set; }
		//标题
		public string Title { get; set; }
		//video下载url
		public string videoUrl { get; set; }
		//image下载url
		public string ImgUrl { get; set; }
	}
}
