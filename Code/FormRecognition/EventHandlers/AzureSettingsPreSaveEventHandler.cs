using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Services.Objects;

namespace FormRecognition.EventHandlers
{
	[kCura.EventHandler.CustomAttributes.Description("Pre Save EventHandler")]
	[System.Runtime.InteropServices.Guid("d211be5a-b6b6-455c-a5ec-202dbdbb3c27")]
	public class AzureSettingsPreSaveEventHandler : kCura.EventHandler.PreSaveEventHandler
	{
		public override Response Execute()
		{

			//Construct a response object with default values.
			kCura.EventHandler.Response retVal = new kCura.EventHandler.Response();
			retVal.Success = true;
			retVal.Message = string.Empty;
			try
			{
				ISecretStore secretStore = this.Helper.GetSecretStore();

				AzureSettings azureSettings = new AzureSettings();


				//Instantiate a Secret object
				Secret secretToWrite = new Secret();

				//Instantiate the Data property of the secret as a Dictionary
				secretToWrite.Data = new Dictionary<string, string>();

				//get values from form 
				string cognitiveServicesKey = (string)this.ActiveArtifact.Fields[Guids.AzureSettings.COGNTIVIE_SERVICES_KEY_FIELD.ToString()].Value.Value;
				string cognitiveServicesEndpoint = (string)this.ActiveArtifact.Fields[Guids.AzureSettings.COGNTIVIE_SERVICES_END_POINT_FIELD.ToString()].Value.Value;
				string storageAccountName = (string)this.ActiveArtifact.Fields[Guids.AzureSettings.STORAGE_ACCOUNT_NAME_FIELD.ToString()].Value.Value;
				string storageAccountKey = (string)this.ActiveArtifact.Fields[Guids.AzureSettings.STORAGE_ACCOUNT_KEY_FIELD.ToString()].Value.Value;

				//Set the value
				secretToWrite.Data.Add(azureSettings.CognitiveServicesKeySecretName, cognitiveServicesKey);
				secretToWrite.Data.Add(azureSettings.CognitiveServicesEndpointSecretName, cognitiveServicesEndpoint);
				secretToWrite.Data.Add(azureSettings.StorageAccountNameSecretName, storageAccountName);
				secretToWrite.Data.Add(azureSettings.StorageAccountKeySecretName, storageAccountKey);

				//clear values so they are not saved to object
				this.ActiveArtifact.Fields[Guids.AzureSettings.STORAGE_ACCOUNT_KEY_FIELD.ToString()].Value.Value = "";
				this.ActiveArtifact.Fields[Guids.AzureSettings.STORAGE_ACCOUNT_NAME_FIELD.ToString()].Value.Value = "";
				this.ActiveArtifact.Fields[Guids.AzureSettings.COGNTIVIE_SERVICES_KEY_FIELD.ToString()].Value.Value = "";
				this.ActiveArtifact.Fields[Guids.AzureSettings.COGNTIVIE_SERVICES_END_POINT_FIELD.ToString()].Value.Value = "";


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
				retVal.Add(new kCura.EventHandler.Field(Guids.AzureSettings.STORAGE_ACCOUNT_KEY_FIELD));
				retVal.Add(new kCura.EventHandler.Field(Guids.AzureSettings.STORAGE_ACCOUNT_NAME_FIELD));
				retVal.Add(new kCura.EventHandler.Field(Guids.AzureSettings.COGNTIVIE_SERVICES_KEY_FIELD));
				retVal.Add(new kCura.EventHandler.Field(Guids.AzureSettings.COGNTIVIE_SERVICES_END_POINT_FIELD));

				return retVal;
			}
		}
	}
}