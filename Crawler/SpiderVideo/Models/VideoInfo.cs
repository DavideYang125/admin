using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Spider.Models
{
	public class VideoInfo
	{
		public string Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string Extractor { get; set; }

		public string Format { get; set; }

		public string FullTitle { get; set; }

		public string WebPage { get; set; }

		[JsonProperty("_filename")]
		public string FileName { get; set; }

		public string Thumbnail { get; set; }

		public string Mp3File { get; set; }

		public string LrcFile { get; set; }

		public string SrtFile { get; set; }

		public DateTime UploadDate { get; set; }

		public string Url { get; set; }
	}
}
