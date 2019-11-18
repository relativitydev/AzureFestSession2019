using FormRecognition.Objects;
using kCura.EventHandler;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FormRecognition
{
	[kCura.EventHandler.CustomAttributes.Description("Console EventHandler for Kramerica Model Object")]
	[System.Runtime.InteropServices.Guid("a97b45db-0173-45ae-adc5-6350653fe415")]
	public class ModelConsoleEventHandler : kCura.EventHandler.ConsoleEventHandler
	{

		public override kCura.EventHandler.Console GetConsole(PageEvent pageEvent)
		{
			kCura.EventHandler.Console returnConsole = new kCura.EventHandler.Console()
			{ Items = new List<IConsoleItem>(), Title = "Azure Form Recognition" };
			;

			returnConsole.Items.Add(new ConsoleButton() { Name = "Submit Documents", DisplayText = "Submit to Train", Enabled = true, RaisesPostBack = true, ToolTip = "Submit saved search to Azure Form Recognizer service for model training" });
			returnConsole.AddRefreshLinkToConsole().Enabled = true;
			return returnConsole;
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{
			SubmitDocuments(consoleButton).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private async Task SubmitDocuments(ConsoleButton consoleButton)
		{
			try
			{
				switch (consoleButton.Name)
				{
					//Handle each Button's functionality
					case "Submit Documents":
						int workspaceId = this.Helper.GetActiveCaseID();
						int modelId = this.ActiveArtifact.ArtifactID;
						Model model = new Model();

						//Get Documents In SavedSearch
						int savedSearchId = (int)this.ActiveArtifact.Fields[Guids.Model.SAVED_SEARCH_FIELD.ToString()].Value.Value;
						await model.ReadDocumentsInSavedSeach(Helper.GetServicesManager(), workspaceId, savedSearchId);

						//Could have made function shared with the other event handler but left here for demo
						Relativity.API.IDBContext workspaceContext = Helper.GetDBContext(workspaceId);
						string documentLocation = string.Empty;
						List<string> documentLocations = new List<string>();
						foreach (int documentArtifactId in model.DocsInSearch)
						{
							string sql = @"SELECT [Location] FROM [file] WITH(NOLOCK) WHERE [DocumentArtifactId] = @documentArtifactID AND [Type] = 0";
							SqlParameter documentArtifactIdParam = new SqlParameter("@documentArtifactID", SqlDbType.Int);
							documentArtifactIdParam.Value = documentArtifactId;
							documentLocation = workspaceContext.ExecuteSqlStatementAsScalar<String>(sql, new SqlParameter[] { documentArtifactIdParam });
							documentLocations.Add(documentLocation);
						}

						//get secrets
						AzureSettings azureSettings = new AzureSettings();

						ISecretStore secretStore = this.Helper.GetSecretStore();
						Secret secret = secretStore.Get(azureSettings.SecretPath);
						string congnitiveServicesKey = secret.Data[azureSettings.CognitiveServicesKeySecretName];
						string congnitiveServicesEndPoint = secret.Data[azureSettings.CognitiveServicesEndpointSecretName];
						string storageAccountKey = secret.Data[azureSettings.StorageAccountKeySecretName];
						string storageAccountName = secret.Data[azureSettings.StorageAccountNameSecretName];


						//upload documents to New Container 
						AzureStorageService azureStorageService = new FormRecognition.AzureStorageService(storageAccountKey, storageAccountName);
						Microsoft.Azure.Storage.Blob.CloudBlobClient client = azureStorageService.GetClient();
						string containerPrefix = workspaceId.ToString() + "-" + modelId.ToString();
						string sasUrl = azureStorageService.UploadFiles(client, documentLocations, containerPrefix);
						string containerName = azureStorageService.ContainerName;

						//Train model
						AzureFormRecognitionService azureFormRecognitionService = new FormRecognition.AzureFormRecognitionService(congnitiveServicesKey, congnitiveServicesEndPoint, Helper);
						Guid modeld = await azureFormRecognitionService.TrainModelAsync(azureFormRecognitionService.GetClient(), sasUrl);
						model.ModelGuid = modeld;
						model.SasUrl = sasUrl;

						//Update Relativity with data returned
						await model.UpdateRelativity(Helper.GetServicesManager(), workspaceId, modelId);

						break;
				}
			}
			catch (Exception e)
			{
				Helper.GetLoggerFactory().GetLogger().LogError(e, "Submit Documents Error");
				throw;
			}
		}

		/// <summary>
		///     The RequiredFields property tells Relativity that your event handler needs to have access to specific fields that
		///     you return in this collection property
		///     regardless if they are on the current layout or not. These fields will be returned in the ActiveArtifact.Fields
		///     collection just like other fields that are on
		///     the current layout when the event handler is executed.
		/// </summary>
		public override FieldCollection RequiredFields
		{
			get
			{
				kCura.EventHandler.FieldCollection retVal = new kCura.EventHandler.FieldCollection();
				retVal.Add(new kCura.EventHandler.Field(Guids.Model.SAVED_SEARCH_FIELD));
				return retVal;
			}
		}
	}
}