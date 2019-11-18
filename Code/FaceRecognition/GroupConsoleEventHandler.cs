using FaceRecognition.Helpers;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FaceRecognition
{
	[Description("Face Recognition: FaceGroup Console EventHandler")]
	[System.Runtime.InteropServices.Guid("d720fbf6-c0cb-4b3f-ac6e-7f5cb79d12d1")]
	public class GroupConsoleEventHandler : ConsoleEventHandler
	{
		private static IFaceClient _client;
		private const string _CREATE_GROUP = "Create Group";
		private const string _DELETE_GROUP = "Delete Group";
		private const string _TRAIN_GROUP = "Train Group";
		private const int _THREAD_DELAY = 3000;

		public override Console GetConsole(PageEvent pageEvent)
		{
			// Update Security Protocol
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			//Create Console Event Handler
			Console returnConsole = new Console() { Items = new List<IConsoleItem>(), Title = "Face Recognition: FaceGroup" };
			returnConsole.Items.Add(new ConsoleButton() { Name = _CREATE_GROUP, DisplayText = _CREATE_GROUP, Enabled = true, RaisesPostBack = true, ToolTip = "Create a new Azure Face Recognizer FaceGroup" });
			returnConsole.Items.Add(new ConsoleButton() { Name = _DELETE_GROUP, DisplayText = _DELETE_GROUP, Enabled = true, RaisesPostBack = true, ToolTip = "This deletes an Azure Face Recognizer FaceGroup (TEMPORARY)" });
			returnConsole.Items.Add(new ConsoleButton() { Name = _TRAIN_GROUP, DisplayText = _TRAIN_GROUP, Enabled = true, RaisesPostBack = true, ToolTip = "Train the Azure Face Recognizer FaceGroup" });

			//Authenticate Azure Service
			_client = Methods.AuthenticateService(this.Helper.GetSecretStore());
			returnConsole.AddRefreshLinkToConsole().Enabled = true;

			return returnConsole;
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{
			ManageGroup(consoleButton).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private async Task ManageGroup(ConsoleButton consoleButton)
		{
			// Update Security Protocol
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			switch (consoleButton.Name)
			{
				case _CREATE_GROUP:
					int nameArtifactId = GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionGroup.NAME);
					string groupId = await CreatePersonGroup(ActiveArtifact.Fields[nameArtifactId].Value.Value.ToString());
					await SetGroupId(groupId);
					await SetGroupStatus(Constant.Group.GROUP_CREATION_STATUS_DESC, true);
					break;
				case _DELETE_GROUP:
					int artifactIdByGuidGroupId = GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionGroup.GROUP_ID);
					await DeletePersonGroup(artifactIdByGuidGroupId);
					await SetGroupId();
					await SetGroupStatus(Constant.Group.GROUP_DELETION_STATUS_DESC, true);
					break;
				case _TRAIN_GROUP:
					int groupIdArtifactId = GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionGroup.GROUP_ID);
					bool trainingResult = await TrainGroup(ActiveArtifact.Fields[groupIdArtifactId].Value.Value.ToString());
					await SetGroupStatus(Constant.Group.GROUP_TRAINING_STATUS_DESC, trainingResult);
					break;
			}
		}


		private async static Task<bool> TrainGroup(string groupId = null)
		{
			await _client.PersonGroup.TrainAsync(groupId);
			TrainingStatus status = await _client.PersonGroup.GetTrainingStatusAsync(groupId);
			while (status.Status == TrainingStatusType.Running || status.Status == TrainingStatusType.Nonstarted)
			{
				await Task.Delay(_THREAD_DELAY);
				status = await _client.PersonGroup.GetTrainingStatusAsync(groupId);
			}
			return status.Status == TrainingStatusType.Succeeded;
		}

		private async Task SetGroupId(string groupId = null)
		{
			using (IObjectManager objectManager = Helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
			{
				RelativityObjectRef relativityObject = new RelativityObjectRef { ArtifactID = ActiveArtifact.ArtifactID };
				FieldRefValuePair fieldValuePair = new FieldRefValuePair
				{
					Field = new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionGroup.GROUP_ID },
					Value = groupId
				};

				UpdateRequest updateRequest = new UpdateRequest
				{
					Object = relativityObject,
					FieldValues = new List<FieldRefValuePair> { fieldValuePair }
				};

				await objectManager.UpdateAsync(Helper.GetActiveCaseID(), updateRequest);
			}
		}

		private async Task SetGroupStatus(string status, bool trainingStatus)
		{
			string statusDetail = trainingStatus ? Constant.Group.COMPLETE : Constant.Group.INCOMPLETE;

			using (IObjectManager objectManager = Helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
			{
				RelativityObjectRef relativityObject = new RelativityObjectRef { ArtifactID = ActiveArtifact.ArtifactID };
				FieldRefValuePair fieldValuePair = new FieldRefValuePair
				{
					Field = new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionGroup.TRAINING_STATUS },
					Value = status + statusDetail
				};

				UpdateRequest updateRequest = new UpdateRequest
				{
					Object = relativityObject,
					FieldValues = new List<FieldRefValuePair> { fieldValuePair }
				};

				await objectManager.UpdateAsync(Helper.GetActiveCaseID(), updateRequest);
			}
		}

		private async Task<string> CreatePersonGroup(string groupName)
		{
			groupName = Helpers.Methods.ModifyName(groupName);
			await _client.PersonGroup.CreateAsync(groupName, groupName, "", RecognitionModel.Recognition02);
			return groupName;
		}

		private async Task DeletePersonGroup(int artifactIdByGuid)
		{
			await _client.PersonGroup.DeleteAsync(ActiveArtifact.Fields[artifactIdByGuid].Value.Value.ToString());
		}

		public override FieldCollection RequiredFields
		{
			get
			{
				FieldCollection retVal = new FieldCollection();
				return retVal;
			}
		}
	}
}