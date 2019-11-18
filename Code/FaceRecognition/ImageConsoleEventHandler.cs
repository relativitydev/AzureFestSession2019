using FaceRecognition.Helpers;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
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
using Console = kCura.EventHandler.Console;

namespace FaceRecognition
{
	[Description("Face Recognition: Image Console EventHandler")]
	[Guid("c2712fd2-bb39-4ff3-aadb-6402c2868c7a")]
	public class ImageConsoleEventHandler : ConsoleEventHandler
	{
		private static IFaceClient _client;
		private static int _currentWorkspaceArtifactId;
		private const string _BUTTON_NAME = "Identify Faces";

		private IObjectManager CreateObjectManager() => Helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser);

		public override Console GetConsole(PageEvent pageEvent)
		{
			// Update Security Protocol
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			Console returnConsole = new Console() { Items = new List<IConsoleItem>(), Title = "Face Recognition: Image" };
			returnConsole.Items.Add(new ConsoleButton() { Name = _BUTTON_NAME, DisplayText = _BUTTON_NAME, Enabled = true, RaisesPostBack = true, ToolTip = "Identify Faces in Image" });
			returnConsole.AddRefreshLinkToConsole().Enabled = true;

			//Set current Workspace Artifact ID
			_currentWorkspaceArtifactId = Helper.GetActiveCaseID();

			//Authenticate Azure Service
			_client = Methods.AuthenticateService(this.Helper.GetSecretStore());

			return returnConsole;
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{
			InternalButtonClick(consoleButton).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public override FieldCollection RequiredFields => new FieldCollection();

		public async Task InternalButtonClick(ConsoleButton consoleButton)
		{
			// Update Security Protocol
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			switch (consoleButton.Name)
			{
				case _BUTTON_NAME:
					//Get Group ID
					int groupArtifactId = (int)ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionImage.GROUP)].Value.Value;
					string groupIdValue = await GetGroupIdValue(_currentWorkspaceArtifactId, groupArtifactId);

					//Get Current Image Location
					int imageArtifactId = (int)ActiveArtifact.Fields[GetArtifactIdByGuid(Constant.Guids.Field.FaceRecognitionImage.IMAGE)].Value.Value;
					string imageLocation = GetFileLocation(_currentWorkspaceArtifactId, imageArtifactId);

					//Identify People in Image for the select Group
					Dictionary<string, double> personList = await IdentifyImage(imageLocation, groupIdValue);

					//Match People In Group
					await MatchPeople(_currentWorkspaceArtifactId, personList, ActiveArtifact.ArtifactID);

					break;
			}
		}

		private async Task MatchPeople(int workspaceArtifactId, Dictionary<string, double> personList, int imageArtifactId)
		{
			string status = Constant.Person.IMAGE_DETECTION_STATUS_DESC;

			if (personList.Any())
			{
				List<Person> detectedPeople = personList.Select(CreatePerson).ToList();

				//Mass Create Results
				await CreateResults(workspaceArtifactId, detectedPeople, imageArtifactId);

				//Set Image Status
				await SetImageStatus(workspaceArtifactId, imageArtifactId, Constant.Image.RESULTS_STATUS_DESC + Constant.Image.COMPLETE);
			}
			else
			{
				status += Constant.Person.COMPLETE + "; " + Constant.Person.NO_PEOPLE_DETECTED;
			}
		}

		private async Task CreateResults(int workspaceArtifactId, List<Person> detectedPeople, int imageArtifactId)
		{
			// Sets the fields to populate.
			MassCreateResult results = null;

			List<FieldRef> fields = new List<FieldRef>
			{
				new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionResults.NAME },
				new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionResults.PERSON_NAME },
				new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionResults.CONFIDENCE},
				new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionResults.RESULTS}
			};

			// Sets the values in the order that the fields provided.
			List<List<object>> fieldValues = detectedPeople.Select(person => new List<object>
			{
				Guid.NewGuid().ToString(),
				person.Name,
				person.Confidence,
				new RelativityObjectRef() { ArtifactID = imageArtifactId }
			}).ToList();

			using (IObjectManager objectManager = CreateObjectManager())
			{
				MassCreateRequest massCreateRequest = new MassCreateRequest
				{
					ObjectType = new ObjectTypeRef { Guid = Constant.Guids.ObjectType.FACE_RECOGNITION_RESULTS },
					Fields = fields,
					ValueLists = fieldValues
				};

				results = await objectManager.CreateAsync(workspaceArtifactId, massCreateRequest);
			}
		}

		private static Person CreatePerson(KeyValuePair<string, double> person)
		{
			return new Person { Name = person.Key, Confidence = person.Value };
		}

		private string GetFileLocation(int workspaceArtifactId, int imageArtifactId)
		{
			//Set DB Context to current Workspace
			IDBContext workspaceContext = Helper.GetDBContext(workspaceArtifactId);

			string sql = @"SELECT [Location] FROM [EDDSDBO].[File] WITH(NOLOCK) WHERE [DocumentArtifactId] = @documentArtifactID AND [Type] = 0";
			SqlParameter documentArtifactIdParam = new SqlParameter("@documentArtifactID", SqlDbType.Int) { Value = imageArtifactId };
			string fileLocation = workspaceContext.ExecuteSqlStatementAsScalar<string>(sql, documentArtifactIdParam);

			return fileLocation;
		}

		private async static Task<Dictionary<string, double>> IdentifyImage(string imagePath, string groupId)
		{
			Dictionary<string, double> nameList = new Dictionary<string, double>();

			using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
			{
				IList<DetectedFace> faces = await _client.Face.DetectWithStreamAsync(stream, true, false, null, RecognitionModel.Recognition02);

				if (faces.Any())
				{
					List<Guid> faceIds = faces.Select(face => face.FaceId.Value).ToList();
					IList<IdentifyResult> results = await _client.Face.IdentifyAsync(faceIds, groupId);

					Person[] people = await Task.WhenAll(results
						.Where(identifyResult => identifyResult.Candidates.Any())
						.Select(async identifyResult =>
					{
						IdentifyCandidate candidate = identifyResult.Candidates.First();
						Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person person = await _client.PersonGroupPerson.GetAsync(groupId, candidate.PersonId);
						return new Person()
						{
							Confidence = candidate.Confidence,
							Name = person.Name
						};
					}
							)
						);
					nameList = people.ToDictionary(person => person.Name, person => person.Confidence);
				}
			}
			return nameList;
		}

		public async Task<string> GetGroupIdValue(int workspaceArtifactId, int groupArtifactId)
		{
			using (IObjectManager objectManager = CreateObjectManager())
			{
				List<FieldRef> fieldRefs = new List<FieldRef> { new FieldRef { Guid = Constant.Guids.Field.FaceRecognitionGroup.GROUP_ID } };

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

		private async Task SetImageStatus(int workspaceArtifactId, int imageArtifactId, string status)
		{
			using (IObjectManager objectManager = CreateObjectManager())
			{
				//Set Status field for current Image
				RelativityObjectRef relativityObject = new RelativityObjectRef { ArtifactID = imageArtifactId };
				FieldRefValuePair fieldValuePair = new FieldRefValuePair
				{
					Field = new FieldRef() { Guid = Constant.Guids.Field.FaceRecognitionImage.STATUS },
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


		private class Person
		{
			public string Name { get; set; }
			public double Confidence { get; set; }
		}
	}
}