using System;
using System.Collections.Generic;
using System.Net;
using kCura.EventHandler;
using Relativity.API;

namespace FaceRecognition
{
	[kCura.EventHandler.CustomAttributes.Description("Post Install EventHandler")]
	[System.Runtime.InteropServices.Guid("f9f0634a-0f44-4c6b-bde9-b839bf4fddce")]
	public class PostInstallEventHandler : kCura.EventHandler.PostInstallEventHandler
	{
		public override Response Execute()
		{
			// Update Security Protocol
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			//Construct a response object with default values.
			Response retVal = new Response { Success = true, Message = string.Empty };

			try
			{
				AzureSettings azureSettings = new AzureSettings();

				//Get the Secret Store interface using a helper
				ISecretStore secretStore = this.Helper.GetSecretStore();

				//Instantiate a Secret object
				Secret secretToWrite = new Secret();

				//Instantiate the Data property of the secret as a Dictionary
				secretToWrite.Data = new Dictionary<string, string>();

				//Set the username value
				secretToWrite.Data.Add(azureSettings.FaceRecognitionKeySecretName, ""); //TODO: Add your own credentials here

				//Set the password value
                secretToWrite.Data.Add(azureSettings.FaceRecognitionEndpointSecretName, ""); //TODO: Add your own credentials here

				//Write the secret to the Secret Store
				secretStore.Set(azureSettings.SecretPath, secretToWrite);

			}
			catch (Exception ex)
			{
				//Change the response Success property to false to let the user know an error occurred
				retVal.Success = false;
				retVal.Message = ex.ToString();
			}

			return retVal;
		}
	}
}