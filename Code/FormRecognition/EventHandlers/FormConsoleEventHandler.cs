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
	[kCura.EventHandler.CustomAttributes.Description("Console EventHandler for Kramerica Form Object")]
	[System.Runtime.InteropServices.Guid("f8ea90a5-c890-4bd7-ad0b-2395edc16f0d")]
	public class FormConsoleEventHandler : kCura.EventHandler.ConsoleEventHandler
	{

		public override kCura.EventHandler.Console GetConsole(PageEvent pageEvent)
		{
			kCura.EventHandler.Console returnConsole = new kCura.EventHandler.Console()
			{ Items = new List<IConsoleItem>(), Title = "Azure Form Recognition" };
			;

			returnConsole.Items.Add(new ConsoleButton() { Name = "Extract Info", DisplayText = "Extract Form Data", Enabled = true, ToolTip = "Submit document to Azure Form Recognizer for data extraction", RaisesPostBack = true });

			return returnConsole;
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{
			ExtractInfo(consoleButton).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private async Task ExtractInfo(ConsoleButton consoleButton)
		{
			switch (consoleButton.Name)
			{
				//Handle each Button's functionality
				case "Extract Info":
					int workspaceID = this.Helper.GetActiveCaseID();
					int invoiceArtifactID = this.ActiveArtifact.ArtifactID;

					Guid modelGuid = Guid.Empty;
					string documentLocation = String.Empty;

					//Get Model and Document could be made a function to get doc location, but keeping for demo
					int modelArtifactId = (int)this.ActiveArtifact.Fields[Guids.Invoice.TRAINING_MODEL.ToString()].Value.Value;
					int documentArtifactId = (int)this.ActiveArtifact.Fields[Guids.Invoice.DOCUMENT.ToString()].Value.Value;

					if ((modelArtifactId > 0) && (documentArtifactId > 0))
					{
						Model model = new Model();
						await model.ReadModelGuid(Helper.GetServicesManager(), workspaceID, modelArtifactId);
						modelGuid = model.ModelGuid;

						IDBContext workspaceContext = Helper.GetDBContext(workspaceID);
						string sql = @"SELECT [Location] FROM [file] WITH(NOLOCK) WHERE [DocumentArtifactId] = @documentArtifactID AND [Type] = 0";
						SqlParameter documentArtifactIdParam = new SqlParameter("@documentArtifactID", SqlDbType.Int);
						documentArtifactIdParam.Value = documentArtifactId;
						documentLocation = workspaceContext.ExecuteSqlStatementAsScalar<String>(sql, new SqlParameter[] { documentArtifactIdParam });

					}

					//get secrets
					AzureSettings azureSettings = new AzureSettings();
					ISecretStore secretStore = this.Helper.GetSecretStore();
					Secret secret = secretStore.Get(azureSettings.SecretPath);
					string congnitiveServicesKey = secret.Data[azureSettings.CognitiveServicesKeySecretName];
					string congnitiveServicesEndPoint = secret.Data[azureSettings.CognitiveServicesEndpointSecretName];

					AzureFormRecognitionService azureFormRecognitionService = new FormRecognition.AzureFormRecognitionService(congnitiveServicesKey, congnitiveServicesEndPoint, Helper);

					Microsoft.Azure.CognitiveServices.FormRecognizer.Models.AnalyzeResult results = await azureFormRecognitionService.AnalyzeForm(azureFormRecognitionService.GetClient(), modelGuid, documentLocation);

					Invoice invoice = new Invoice();

					invoice = invoice.ConvertAnalyzeResultToInvoice(results, Helper);
					IServicesMgr mgr = Helper.GetServicesManager();
					await invoice.UpdateRelativity(mgr, workspaceID, invoiceArtifactID);

					break;
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
				FieldCollection retVal = new FieldCollection();
				retVal.Add(new Field(Guids.Invoice.NAME_FIELD));
				retVal.Add(new Field(Guids.Invoice.DOCUMENT));
				retVal.Add(new Field(Guids.Invoice.TRAINING_MODEL));
				return retVal;
			}
		}
	}
}