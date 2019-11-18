using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;

namespace FormRecognition
{
	public class AzureSettings
	{
		public readonly string SecretPath = @"azure/settings";
		public readonly string CognitiveServicesKeySecretName = "cognitive_services_key";
		public readonly string CognitiveServicesEndpointSecretName = "cognitive_services_endpoint";
		public readonly string StorageAccountNameSecretName = "storage_account_name";
		public readonly string StorageAccountKeySecretName = "storage_account_key";


		public string CognitiveSerivcesKey { get; set; }
		public string CognitiveSerivcesEndPoint { get; set; }
		public string StorageAccountName { get; set; }
		public string StorageAccountKey { get; set; }

	}

}
