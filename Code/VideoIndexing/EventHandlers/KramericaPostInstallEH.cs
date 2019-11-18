using kCura.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VideoIndexing;

namespace VideoIndexing.EventHandlers
{
	[kCura.EventHandler.CustomAttributes.Description("Kramerica Video Indexing Post Install Event Handler")]
	[System.Runtime.InteropServices.Guid("a7aefbc0-a91b-454c-94bd-f90d2a5ee53e")]
	public class KramericaPostInstallEH : PostInstallEventHandler
	{
		private Relativity.API.IAPILog _logger;

		public override Response Execute()
		{
			Response retVal = new Response();
			retVal.Message = "";
			retVal.Success = true;

			bool success = CreateJobQueue();
			if (!success)
			{
				retVal.Message = "Failed to create job queue.";
				retVal.Success = false;
			}

			return retVal;
		}

		private bool CreateJobQueue()
		{
			bool success = false;

			try
			{
				string sql = @"
                IF NOT EXISTS (SELECT 'true' FROM [INFORMATION_SCHEMA].[TABLES] WHERE [TABLE_NAME] = 'KramericaVideoIndexQueue' and  [TABLE_SCHEMA] = 'eddsdbo')
                BEGIN
	                CREATE TABLE [eddsdbo].[KramericaVideoIndexQueue](
		                [WorkspaceArtifactID] [int] NOT NULL,
		                [JobArtifactID] [int] NOT NULL,
		                [DocumentArtifactID] [int] NOT NULL,
		                [AgentID] [int] NULL,
                        [Status] [int] DEFAULT (0) NOT NULL,
		                [CreateDateTime] [datetime] DEFAULT GETDATE() NOT NULL)
                END";
				Helper.GetDBContext(-1).ExecuteNonQuerySQLStatement(sql);
				success = true;
			}
			catch (Exception ex)
			{
				LogError(ex);
			}


			return success;
		}

		private void LogError(Exception ex, [CallerMemberName] string caller = null)
		{
			_logger = _logger ?? Helper.GetLoggerFactory().GetLogger().ForContext<KramericaPostInstallEH>();
			_logger.LogError(ex, "{caller} Error: {message}", caller, ex?.Message);
		}
	}
}
