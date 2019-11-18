using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VideoIndexing.Agents;

namespace VideoIndexing
{
	[kCura.Agent.CustomAttributes.Name("Kramerica Video Indexing Worker")]
	[System.Runtime.InteropServices.Guid("3a74d357-31f1-4445-8afb-8712ad2bb366")]
	[System.ComponentModel.Description("Kramerica Video Indexing Worker")]
	public class KramericaVideoIndexingWorker : AgentBase
	{
		private Relativity.API.IAPILog _logger;

		#region Properties
		private string _indexerApiUrl { get; set; }
		private string _indexerApiKey { get; set; }
		#endregion
		public override string Name
		{
			get
			{
				return "Kramerica Video Indexing Worker";
			}
		}


		public override void Execute()
		{
			bool success = Task.Run(async () => await ExecuteAsync()).Result;
		}


		private async Task<bool> ExecuteAsync()
		{
			bool executedSuccessfully = true;
			try
			{
				if (GetConfigurationValues())
				{
					_indexerApiUrl = "https://api.videoindexer.ai";
					//Retrieve _indexerApiKey from secret store
					_indexerApiKey = ""; //Add your own credentials here

					//Get job details
					IndexJob indexJob = new IndexJob();
					indexJob = GetSingleJob();


					if (indexJob.Success)
					{
						await UpdateStatus(indexJob, "In Progress");

						//Get job document details
						DocProperty doc = new DocProperty();
						doc = await GetDocProperty(indexJob.WorkspaceArtifactID, indexJob.DocumentArtifactID);

						if (doc.Success)
						{
							//Launch video for indexing
							VideoIndex videoIndex = new VideoIndex(doc.Path, doc.Filename, doc.Begdoc, _indexerApiUrl, _indexerApiKey, _logger, Helper);
							VideoIndexResult videoIndexResult = await videoIndex.Sample();
							//Update job with index details.  We will use these details later in the custom page.
							await WriteValuesToIndexJob(indexJob, videoIndexResult);
							await WriteValuesToDocumentObject(indexJob, videoIndexResult);
							//Write transcript to document field
						}
						CleanupQueue(indexJob);
						await UpdateStatus(indexJob, "Complete");
					}
				}
				const int maxMessageLevel = 10;
				RaiseMessage("Completed.", maxMessageLevel);
			}
			catch (Exception ex)
			{
				LogError(ex);
				executedSuccessfully = false;
			}
			return executedSuccessfully;
		}

		private bool GetConfigurationValues()
		{
			bool success = false;
			try
			{
				//I would normally store these in the Secret Store, Encrypted table, or encrypted values in an instance setting object.
				//retrieve values and set properties (or create a dictionary with key value pairs).
				_indexerApiUrl = "https://api.videoindexer.ai";
				_indexerApiKey = ""; //TODO: Add your own credentials here
				success = true;
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
			return success;
		}

		private IndexJob GetSingleJob()
		{
			IndexJob indexJob = new IndexJob();
			indexJob.DocumentArtifactID = 0;
			indexJob.JobArtifactID = 0;
			indexJob.WorkspaceArtifactID = 0;

			try
			{
				//Tag single doc to process
				string tagSql = @"
														UPDATE TOP (1) [eddsdbo].[KramericaVideoIndexQueue]
														SET [AgentID] = @agentIdParam, [Status] = 1
														WHERE [AgentID] IS NULL
														AND [Status] = 0";
				SqlParameter agentIdParam = new SqlParameter("@agentIdParam", System.Data.SqlDbType.Int);
				agentIdParam.Value = AgentID;
				Helper.GetDBContext(-1).ExecuteNonQuerySQLStatement(tagSql, new SqlParameter[] { agentIdParam });

				string getJobSql = @"
																SELECT TOP 1 * FROM [eddsdbo].[KramericaVideoIndexQueue]
																WHERE [AgentID] = @agentIdParam 
																AND [Status] = 1";
				DataTable tbl = new DataTable();
				tbl = Helper.GetDBContext(-1).ExecuteSqlStatementAsDataTable(getJobSql, new SqlParameter[] { agentIdParam });

				if (tbl.Rows.Count > 0)
				{
					indexJob.DocumentArtifactID = Convert.ToInt32(tbl.Rows[0]["DocumentArtifactID"].ToString()) as int? ?? 0;
					indexJob.JobArtifactID = Convert.ToInt32(tbl.Rows[0]["JobArtifactID"].ToString()) as int? ?? 0;
					indexJob.WorkspaceArtifactID = Convert.ToInt32(tbl.Rows[0]["WorkspaceArtifactID"].ToString()) as int? ?? 0;
				}

				indexJob.Success = (indexJob.DocumentArtifactID > 0 && indexJob.JobArtifactID > 0 && indexJob.WorkspaceArtifactID > 0);
			}
			catch (Exception ex)
			{
				LogError(ex);
			}

			return indexJob;
		}



		private async Task<DocProperty> GetDocProperty(int workspaceArtifactID, int documentArtifactID)
		{
			DocProperty doc = new DocProperty();
			const string fileNameFieldName = "File Name";
			try
			{
				using (IObjectManager manager = this.Helper.GetServicesManager().CreateProxy<IObjectManager>(Relativity.API.ExecutionIdentity.System))
				{

					var fieldsToRead = new List<FieldRef>();
					fieldsToRead.Add(new FieldRef() { Name = fileNameFieldName });
					var readRequest = new ReadRequest()
					{
						Object = new RelativityObjectRef { ArtifactID = documentArtifactID },
						Fields = fieldsToRead
					};
					RelativityObject document = (await manager.ReadAsync(workspaceArtifactID, readRequest)).Object;
					if (document != null)
					{
						doc.Begdoc = document.Name;
						doc.Filename = document.FieldValues.First(f => f.Field.Name == fileNameFieldName).Value.ToString();

						string sql = @"
															SELECT [Location] 
															FROM [EDDSDBO].[File]
															WHERE [Type] = 0
															AND [DocumentArtifactID] = @documentArtifactIDParam";
						SqlParameter documentArtifactIDParam = new SqlParameter("@documentArtifactIDParam", SqlDbType.Int);
						documentArtifactIDParam.Value = documentArtifactID;
						doc.Path = Helper.GetDBContext(workspaceArtifactID).ExecuteSqlStatementAsScalar(sql, new SqlParameter[] { documentArtifactIDParam }).ToString() ?? string.Empty;
					}
					doc.Success = !string.IsNullOrEmpty(doc.Path) && !string.IsNullOrEmpty(doc.Filename);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
			return doc;
		}

		private class FieldValue : FieldRefValuePair
		{
			public FieldValue(string fieldName, object fieldValue) : base()
			{
				Field = new FieldRef() { Name = fieldName };
				Value = fieldValue;
			}
		}

		private async Task UpdateStatus(IndexJob indexJob, string status)
		{
			try
			{
				using (IObjectManager manager = this.Helper.GetServicesManager().CreateProxy<IObjectManager>(Relativity.API.ExecutionIdentity.System))
				{
					var toUpdate = new RelativityObjectRef { ArtifactID = indexJob.JobArtifactID };
					var fieldValuePair = new FieldValue("Status", status);
					var updateRequest = new UpdateRequest
					{
						Object = toUpdate,
						FieldValues = new List<FieldRefValuePair> { fieldValuePair }
					};
					await manager.UpdateAsync(indexJob.WorkspaceArtifactID, updateRequest);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		private async Task WriteValuesToIndexJob(IndexJob indexJob, VideoIndexResult videoIndexResult)
		{
			try
			{
				using (IObjectManager manager = this.Helper.GetServicesManager().CreateProxy<IObjectManager>(Relativity.API.ExecutionIdentity.System))
				{
					var toUpdate = new RelativityObjectRef { ArtifactID = indexJob.JobArtifactID };
					var fields = new FieldValue[]
					{
						new FieldValue("VideoID", videoIndexResult.VideoID),
						new FieldValue("Video File Name", videoIndexResult.VideoName),
						new FieldValue("DocumentControlNumber", videoIndexResult.ControlNumber)
					};
					var updateRequest = new UpdateRequest { Object = toUpdate, FieldValues = fields };
					await manager.UpdateAsync(indexJob.WorkspaceArtifactID, updateRequest);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		private async Task WriteValuesToDocumentObject(IndexJob indexJob, VideoIndexResult videoIndexResult)
		{
			try
			{
				using (IObjectManager manager = this.Helper.GetServicesManager().CreateProxy<IObjectManager>(Relativity.API.ExecutionIdentity.System))
				{
					var toUpdate = new RelativityObjectRef { ArtifactID = indexJob.DocumentArtifactID };
					var fieldValuePair = new FieldValue("Kramerica Video Transcript", videoIndexResult.Transcript);
					var updateRequest = new UpdateRequest
					{
						Object = toUpdate,
						FieldValues = new List<FieldRefValuePair> { fieldValuePair }
					};
					await manager.UpdateAsync(indexJob.WorkspaceArtifactID, updateRequest);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		private void CleanupQueue(IndexJob indexJob)
		{
			try
			{
				string sql = @"DELETE FROM [eddsdbo].[KramericaVideoIndexQueue] WHERE [WorkspaceArtifactID] = @workspaceArtifactIDParam AND [JobArtifactID] = @jobArtifactIDParam";
				SqlParameter workspaceArtifactIDParam = new SqlParameter("@workspaceArtifactIDParam", SqlDbType.Int) { Value = indexJob.WorkspaceArtifactID };
				SqlParameter jobArtifactIDParam = new SqlParameter("@jobArtifactIDParam", SqlDbType.Int) { Value = indexJob.JobArtifactID };
				Helper.GetDBContext(-1).ExecuteNonQuerySQLStatement(sql, new SqlParameter[] { workspaceArtifactIDParam, jobArtifactIDParam });
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		private void LogError(Exception ex, [CallerMemberName] string caller = null)
		{
			try
			{
				_logger = _logger ?? Helper.GetLoggerFactory().GetLogger().ForContext<KramericaVideoIndexingWorker>();
				_logger.LogError(ex, "{caller} Error: {message}", caller, ex?.Message);
			}
			catch
			{
				RaiseError("_logger failed in Kramerica Video Indexing Worker", ex?.ToString());
			}
		}
	}
}
