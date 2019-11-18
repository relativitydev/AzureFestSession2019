using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
	public class AzureSettings
	{
		public readonly string SecretPath = @"azure/settings";
		public readonly string FaceRecognitionKeySecretName = "cognitive_services_face_recognition_key";
		public readonly string FaceRecognitionEndpointSecretName = "cognitive_services_face_recognition_endpoint";

		public string FaceRecognitionKey { get; set; }
		public string FaceRecognitionEndPoint { get; set; }
	}
}
