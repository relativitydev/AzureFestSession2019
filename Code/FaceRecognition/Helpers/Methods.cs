using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Relativity.API;

namespace FaceRecognition.Helpers
{
	public class Methods
	{
		public static string ModifyName(string name)
		{
			name = name.Replace(" ", "_").ToLower();
			return name;
		}

		public static IFaceClient AuthenticateService(ISecretStore secretStore)
		{
			//Obtain Secrets from Secret Store
			AzureSettings azureSettings = new AzureSettings();
			Secret secret = secretStore.Get(azureSettings.SecretPath);
			string faceRecognitionKey = secret.Data[azureSettings.FaceRecognitionKeySecretName];
			string faceRecognitionEndPoint = secret.Data[azureSettings.FaceRecognitionEndpointSecretName];

			return new FaceClient(new ApiKeyServiceClientCredentials(faceRecognitionKey))
			{
				Endpoint = faceRecognitionEndPoint
			};
		}
	}
}
