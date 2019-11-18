using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoIndexing.Agents
{
	public class VideoIndexResult
	{
		public string VideoID { get; set; }
		public string ControlNumber { get; set; }
		public string VideoName { get; set; }
		public string Transcript { get; set; }
	}
}
