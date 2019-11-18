using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoIndexing.Agents
{
	public class AccountContractSlim
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Location { get; set; }
		public string AccountType { get; set; }
		public string Url { get; set; }
		public string AccessToken { get; set; }
	}
}
