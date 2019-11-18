using kCura.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoIndexing;
using kCura.Relativity.Client;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace VideoIndexing.EventHandlers
{
	[kCura.EventHandler.CustomAttributes.Description("Kramerica Video Indexing Console Event Handler")]
	[System.Runtime.InteropServices.Guid("8b3113a4-729a-4d19-bd1f-f7c8cb18d638")]
	public class KramericaVideoIndexJobConsoleEH : ConsoleEventHandler
	{
		private Relativity.API.IAPILog _logger;
		private ConsoleButton _startButton = new ConsoleButton() { Name = _INSERT_START_BUTTON_NAME, DisplayText = "Start Indexing", RaisesPostBack = true, ToolTip = "Start Indexing" };
		private ConsoleButton _refreshButton = new ConsoleButton() { Name = _INSERT_REFRESH_BUTTON_NAME, DisplayText = "Refresh Page", RaisesPostBack = true, ToolTip = "Refresh Page" };
		private ConsoleButton _videoButton = new ConsoleButton() { Name = _INSERT_LAUNCH_VIDEO_BUTTON_NAME, DisplayText = "Play Indexed Video", RaisesPostBack = true, ToolTip = "Play Indexed Video" };
		private const String _INSERT_START_BUTTON_NAME = "_insertJobButton";
		private const String _INSERT_REFRESH_BUTTON_NAME = "_insertRefreshButton";
		private const String _INSERT_LAUNCH_VIDEO_BUTTON_NAME = "_videoButton";

		public override kCura.EventHandler.Console GetConsole(PageEvent pageEvent)
		{
			kCura.EventHandler.Console retVal = new kCura.EventHandler.Console();
			retVal.ButtonList = GetButtons();
			retVal.Title = "Kramerica Video Indexer Console";

			try
			{
				if (pageEvent == PageEvent.Load)
				{
					kCura.EventHandler.Field myStatusField = this.ActiveArtifact.Fields[ApplicationConstants.INDEX_JOB_STATUS.ToString()];
					string myStatusValue = (string)myStatusField.Value.Value;
					switch (myStatusValue)
					{
						case "Created":
							_startButton.Enabled = true;
							_refreshButton.Enabled = false;
							_videoButton.Enabled = false;
							break;
						case "Complete":
							_startButton.Enabled = false;
							_refreshButton.Enabled = false;
							_videoButton.Enabled = true;
							string customPageUrl = GetCustomPageUrl();
							String playerJavaScript = String.Format("window.open('{0}', '', 'location=no,scrollbars=yes,menubar=no,toolbar=no,status=no,resizable=yes,width=1920,height=1080');", customPageUrl);
							_videoButton.OnClickEvent = playerJavaScript;
							break;
						default:
							_startButton.Enabled = false;
							_refreshButton.Enabled = true;
							_videoButton.Enabled = false;
							break;
					}
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}

			return retVal;
		}

		private List<ConsoleButton> GetButtons()
		{

			return new List<ConsoleButton>()
			{
				_startButton,
				_refreshButton,
				_videoButton
			};
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{
			ManageQueueing(consoleButton).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private async Task ManageQueueing(ConsoleButton consoleButton)
		{
			try
			{
				switch (consoleButton.Name)
				{
					case _INSERT_START_BUTTON_NAME:
						await SetStatus("Queued for Processing");
						InsertJobIntoQueue();
						break;
					case _INSERT_REFRESH_BUTTON_NAME:
						break;
					case _INSERT_LAUNCH_VIDEO_BUTTON_NAME:
						break;
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		private string GetCustomPageUrl()
		{
			string customPageUrl = string.Empty;
			try
			{
				String basePath = this.Application.ApplicationUrl.Substring(0, this.Application.ApplicationUrl.IndexOf("/Case/Mask/"));
				customPageUrl = string.Format("{0}/CustomPages/f46de497-addd-4a3a-bd1a-07ee2ae956e4/VideoPlayer.aspx?AppID={1}&JobID={2}", basePath, Helper.GetActiveCaseID().ToString(), this.ActiveArtifact.ArtifactID.ToString());
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
			return customPageUrl;
		}

		private async Task SetStatus(string status)
		{
			try
			{
				using (IObjectManager manager = this.Helper.GetServicesManager().CreateProxy<IObjectManager>(Relativity.API.ExecutionIdentity.System))
				{
					var toUpdate = new RelativityObjectRef { ArtifactID = ActiveArtifact.ArtifactID };
					var fieldValuePair = new FieldRefValuePair() { Field = new FieldRef() { Guid = ApplicationConstants.INDEX_JOB_STATUS }, Value = status };
					var updateRequest = new UpdateRequest { Object = toUpdate, FieldValues = new FieldRefValuePair[] { fieldValuePair } };
					await manager.UpdateAsync(this.Helper.GetActiveCaseID(), updateRequest);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		private void InsertJobIntoQueue()
		{
			try
			{
				string sql = @"
                                INSERT INTO [eddsdbo].[KramericaVideoIndexQueue] (WorkspaceArtifactID, JobArtifactID, DocumentArtifactID)
                                VALUES (@workspaceArtifactIDParam,@jobArtifactIDParam,@documentArtifactIDParam)";

				SqlParameter workspaceArtifactIDParam = new SqlParameter("@workspaceArtifactIDParam", System.Data.SqlDbType.Int);
				workspaceArtifactIDParam.Value = Helper.GetActiveCaseID();
				SqlParameter jobArtifactIDParam = new SqlParameter("@jobArtifactIDParam", System.Data.SqlDbType.Int);
				jobArtifactIDParam.Value = ActiveArtifact.ArtifactID;
				SqlParameter documentArtifactIDParam = new SqlParameter("@documentArtifactIDParam", System.Data.SqlDbType.Int);
				documentArtifactIDParam.Value = (int)ActiveArtifact.Fields[ApplicationConstants.INDEX_JOB_VIDEO_FILE.ToString()].Value.Value;
				Helper.GetDBContext(-1).ExecuteNonQuerySQLStatement(sql, new SqlParameter[] { workspaceArtifactIDParam, jobArtifactIDParam, documentArtifactIDParam });
			}
			catch (Exception ex)
			{
				LogError(ex);
				throw;
			}
		}

		public override FieldCollection RequiredFields
		{
			get
			{
				kCura.EventHandler.FieldCollection retVal = new kCura.EventHandler.FieldCollection();
				retVal.Add(new kCura.EventHandler.Field(ApplicationConstants.INDEX_JOB_STATUS));
				retVal.Add(new kCura.EventHandler.Field(ApplicationConstants.INDEX_JOB_VIDEO_FILE));
				return retVal;
			}
		}

		private void LogError(Exception ex, [CallerMemberName] string caller = null)
		{
			_logger = _logger ?? Helper.GetLoggerFactory().GetLogger().ForContext<KramericaVideoIndexJobConsoleEH>();
			_logger.LogError(ex, "{caller} Error: {message}", caller, ex?.Message);
		}
	}
}
