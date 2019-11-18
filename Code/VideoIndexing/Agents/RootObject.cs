using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoIndexing.Agents
{
	public class Rootobject
	{
		public string partition { get; set; }
		public string description { get; set; }
		public string privacyMode { get; set; }
		public string state { get; set; }
		public string accountId { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public string userName { get; set; }
		public DateTime created { get; set; }
		public bool isOwned { get; set; }
		public bool isEditable { get; set; }
		public bool isBase { get; set; }
		public int durationInSeconds { get; set; }
		public Summarizedinsights summarizedInsights { get; set; }
		public Video[] videos { get; set; }
		public Videosrange[] videosRanges { get; set; }
	}
}
