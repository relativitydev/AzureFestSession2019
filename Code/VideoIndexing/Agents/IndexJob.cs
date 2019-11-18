using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoIndexing.Agents
{
	public class IndexJob
	{
		public bool Success { get; set; }
		public int WorkspaceArtifactID { get; set; }
		public int JobArtifactID { get; set; }
		public int DocumentArtifactID { get; set; }
	}
}
