using kCura.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoIndexing;
using kCura.EventHandler.PostExecuteAction;
using kCura.Relativity.Client;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System.Runtime.CompilerServices;

namespace VideoIndexing.EventHandlers
{
	[kCura.EventHandler.CustomAttributes.Description("Kramerica Video Indexing Post Save Event Handler")]
	[System.Runtime.InteropServices.Guid("49416361-141d-43ca-90b1-7f15ac9409cc")]
	public class KramericaVideoIndexJobPostSaveEH : PostSaveEventHandler
	{
		private Relativity.API.IAPILog _logger;
		public override Response Execute()
		{
			Response retVal = new Response();
			bool success = Task.Run(async () => await UpdateStatus()).Result;

			retVal.Message = string.Empty;
			retVal.Success = success;

			if (retVal.Success)
			{
				retVal.Message = "Failed to update status.";
			}

			return retVal;
		}

		private async Task<bool> UpdateStatus()
		{
			bool success = false;
			try
			{
				kCura.EventHandler.Field myStatusField = this.ActiveArtifact.Fields[ApplicationConstants.INDEX_JOB_STATUS.ToString()];
				if (myStatusField.Value.Value == null)
				{
					using (IObjectManager manager = this.Helper.GetServicesManager().CreateProxy<IObjectManager>(Relativity.API.ExecutionIdentity.CurrentUser))
					{
						var toUpdate = new RelativityObjectRef { ArtifactID = ActiveArtifact.ArtifactID };
						var fieldValuePair = new FieldRefValuePair() { Field = new FieldRef() { Guid = ApplicationConstants.INDEX_JOB_STATUS }, Value = "Created" };
						var updateRequest = new UpdateRequest { Object = toUpdate, FieldValues = new FieldRefValuePair[] { fieldValuePair } };
						await manager.UpdateAsync(this.Helper.GetActiveCaseID(), updateRequest);
					}
				}
				success = true;
			}
			catch (Exception ex)
			{
				LogError(ex);
				success = false;
			}
			return success;
		}


		public override FieldCollection RequiredFields
		{
			get
			{
				FieldCollection fc = new FieldCollection();
				return fc;
			}
		}

		private void LogError(Exception ex, [CallerMemberName] string caller = null)
		{
			_logger = _logger ?? Helper.GetLoggerFactory().GetLogger().ForContext<KramericaVideoIndexJobPostSaveEH>();
			_logger.LogError(ex, "{caller} Error: {message}", caller, ex?.Message);
		}
	}
}
