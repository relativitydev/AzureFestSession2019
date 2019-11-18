using System;
using System.IO;
using System.Threading.Tasks;

namespace FormRecognition
{
	public class AzureFormRecognitionService
	{
		private string _cognitiveServicesSubscriptionKey = string.Empty;
		private string _cognitiveEndpoint = string.Empty;
		private Lazy<IAPILog> _logger;

		public AzureFormRecognitionService(string cognitiveServicesKey, string cognitiveServiesEndPoint, IHelper helper)
		{
			_cognitiveServicesSubscriptionKey = cognitiveServicesKey;
			_cognitiveEndpoint = cognitiveServiesEndPoint;
			_logger = new Lazy<IAPILog>(() => helper.GetLoggerFactory().GetLogger().ForContext<AzureFormRecognitionService>());
		}

		public FormRecognizerClient GetClient()
		{
			return new FormRecognizerClient(
				new ApiKeyServiceClientCredentials(_cognitiveServicesSubscriptionKey))
			{
				Endpoint = _cognitiveEndpoint
			};
		}

		public async Task<Guid> TrainModelAsync(IFormRecognizerClient client, string sasUrl)
		{
			ModelResult model = null;
			try
			{
				TrainResult result = await client.TrainCustomModelAsync(new TrainRequest(sasUrl));
				model = await client.GetCustomModelAsync(result.ModelId);
				//Check if successful
			}
			catch (ErrorResponseException e)
			{
				LogError(e);
				return Guid.Empty;
			}
			return model.ModelId;
		}

		public async Task<AnalyzeResult> AnalyzeForm(IFormRecognizerClient client, Guid modelId, string filePath)
		{
			AnalyzeResult result = null;
			//Default to PDF
			string contentType = "application/pdf";

			switch (Path.GetExtension(filePath).ToLower().Replace(".", ""))
			{
				case "pdf":
					contentType = "application/pdf";
					break;
				case "jpg":
					contentType = "image/jpeg";
					break;
				case "png":
					contentType = "image/png";
					break;
				default:
					break;
			}

			try
			{
				using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					result = await client.AnalyzeWithCustomModelAsync(modelId, stream, contentType: contentType);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}

			//used the following to serialize the result object and use for testing
			//var jsonVersion = Newtonsoft.Json.JsonConvert.SerializeObject(result);
			//System.IO.File.WriteAllText(Shared.FormRecognitionService.PATH_TO_PROJECT_ROOT + @"\Code\SampleData\SerializedObjects\AnalyzedResult.json", jsonVersion);

			return result;
		}

		private void LogError(Exception e)
		{
			_logger.Value.LogError(e, nameof(AzureFormRecognitionService));
		}

	}
}
