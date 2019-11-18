using FaceRecognition.Helpers;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FaceRecognition
{
	[Description("Face Recognition: Person Console EventHandler")]
	[Guid("36a3bee7-3abf-417f-b3f4-7043c14b5f19")]
	public class PersonConsoleEventHandler : ConsoleEventHandler
	{
		private static IFaceClient _client;
		private static int _currentWorkspaceArtifactId;
		private const string _ADD_PERSON = "Add Person and Images";

		public override kCura.EventHandler.Console GetConsole(PageEvent pageEvent)
		{
			// Update Security Protocol
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			//Create Console
			kCura.EventHandler.Console returnConsole = new kCura.EventHandler.Console()
			{ Items = new List<IConsoleItem>(), Title = "Face Recognition: Person" };
			returnConsole.Items.Add(new ConsoleButton()
			{
				Name = _ADD_PERSON,
				DisplayText = _ADD_PERSON,
				Enabled = true,
				RaisesPostBack = true,
				ToolTip = "Add Person and Image to Azure Face Recognizer FaceGroup"
			});

			//Set current Workspace Artifact ID
			_currentWorkspaceArtifactId = Helper.GetActiveCaseID();

			//Authenticate Azure Service
			_client = Methods.AuthenticateService(this.Helper.GetSecretStore());
			returnConsole.AddRefreshLinkToConsole().Enabled = true;

			return returnConsole;
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{
			AddPersonAndImages(consoleButton).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private async Task AddPersonAndImages(ConsoleButton consoleButton)
		{
			// Update Security Protocol
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			switch (consoleButton.Name)
			{
				case _ADD_PERSON:
					//Get Group ID
					int groupArtifactId = (int)ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionPerson.GROUP)].Value.Value;
					string groupIdValue = await GetGroupIdValue(_currentWorkspaceArtifactId, groupArtifactId);

					//Create Person in Group
					string personName = ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionPerson.NAME)].Value.Value.ToString();
					personName = Methods.ModifyName(personName);

					//Create person in Azure Group
					Guid personGuid = await CreatePersonInGroup(groupIdValue, personName);

					//Set Person Id on current object
					await SetPersonName(_currentWorkspaceArtifactId, ActiveArtifact.ArtifactID, personGuid);

					//Get List of Image for the Person
					List<string> imageList = GetPersonImageList(_currentWorkspaceArtifactId);

					//Add Image for Person in Group
					await AddPersonImages(groupIdValue, personGuid, personName, imageList);

					//Set Status of Person
					await SetPersonStatus(_currentWorkspaceArtifactId, ActiveArtifact.ArtifactID, "Complete");

					break;
			}
		}

		public async Task<string> GetGroupIdValue(int workspaceArtifactId, int groupArtifactId)
		{
			using (IObjectManager objectManager = Helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
			{
				IEnumerable<FieldRef> fieldRefs = new List<FieldRef> { new FieldRef { Guid = Helpers.Constant.Guids.Field.FaceRecognitionGroup.GROUP_ID } };

				ReadRequest readRequest = new ReadRequest
				{
					Object = new RelativityObjectRef { ArtifactID = groupArtifactId },
					Fields = fieldRefs
				};

				ReadResult readReturnValue = await objectManager.ReadAsync(workspaceArtifactId, readRequest);
				string groupId = readReturnValue.Object.FieldValues.Find(x => x.Field.Guids.Contains(Constant.Guids.Field.FaceRecognitionGroup.GROUP_ID)).Value.ToString();

				return groupId;
			}
		}

		private async Task SetPersonName(int workspaceArtifactId, int personArtifactId, Guid personName)
		{
			using (IObjectManager objectManager = Helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
			{
				//Set PersonID field for current Person
				RelativityObjectRef relativityObject = new RelativityObjectRef { ArtifactID = personArtifactId };
				FieldRefValuePair fieldValuePair = new FieldRefValuePair
				{
					Field = new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionPerson.PERSON_ID },
					Value = personName
				};

				UpdateRequest updateRequest = new UpdateRequest
				{
					Object = relativityObject,
					FieldValues = new List<FieldRefValuePair> { fieldValuePair }
				};

				UpdateResult updateResult = await objectManager.UpdateAsync(workspaceArtifactId, updateRequest);
			}
		}

		private async Task SetPersonStatus(int workspaceArtifactId, int personArtifactId, string status)
		{
			using (IObjectManager objectManager = Helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
			{
				//Set Status field for current Person
				RelativityObjectRef relativityObject = new RelativityObjectRef { ArtifactID = personArtifactId };
				FieldRefValuePair fieldValuePair = new FieldRefValuePair
				{
					Field = new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionPerson.STATUS },
					Value = status
				};

				UpdateRequest updateRequest = new UpdateRequest
				{
					Object = relativityObject,
					FieldValues = new List<FieldRefValuePair> { fieldValuePair }
				};

				UpdateResult updateResult = await objectManager.UpdateAsync(workspaceArtifactId, updateRequest);
			}
		}

		private async static Task<Guid> CreatePersonInGroup(string groupIdValue, string personName)
		{
			//Create Person in Azure Group
			Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person person = await _client.PersonGroupPerson.CreateAsync(groupIdValue, personName);
			return person.PersonId;
		}

		private async static Task AddPersonImages(string groupIdValue, Guid personGuid, string personName, List<string> imagePaths)
		{
			foreach (string imagePath in imagePaths)
			{
				using (Stream stream = File.OpenRead(imagePath))
				{
					await _client.PersonGroupPerson.AddFaceFromStreamAsync(groupIdValue, personGuid, stream, personName);
				}
			}
		}

		private string GetFileLocation(int workspaceArtifactId, int documentArtifactId)
		{
			//Set DB Context to current Workspace
			IDBContext workspaceContext = Helper.GetDBContext(workspaceArtifactId);

			string sql = @"SELECT [Location] FROM [EDDSDBO].[File] WITH(NOLOCK) WHERE [DocumentArtifactId] = @documentArtifactID AND [Type] = 0";
			SqlParameter documentArtifactIdParam = new SqlParameter("@documentArtifactID", SqlDbType.Int) { Value = documentArtifactId };
			string fileLocation = workspaceContext.ExecuteSqlStatementAsScalar<string>(sql, documentArtifactIdParam);
			return fileLocation;
		}

		private List<string> GetPersonImageList(int workspaceArtifactId)
		{
			int[] docArtifactIdList = (int[])ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionPerson.IMAGES)].Value.Value;
			return docArtifactIdList.Select(docArtifactId => GetFileLocation(workspaceArtifactId, docArtifactId)).ToList();
		}

		public override FieldCollection RequiredFields => new FieldCollection();
	}
}