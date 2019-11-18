using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace FormRecognition.Objects
{
	public class Model
	{

		public Guid ModelGuid { get; set; }
		public string SasUrl { get; set; }
		public List<int> DocsInSearch { get; set; }

		public async Task<int> CreateRelativity(IServicesMgr serviceManager, int workspaceId, string name, int savedSearchId)
		{
			int returnValue = 0;
			using (IObjectManager objectManager = serviceManager.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{

				List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>
				{
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Model.NAME_FIELD },
						Value = name
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Model.SAVED_SEARCH_FIELD },
						Value = new RelativityObjectRef { ArtifactID = savedSearchId }
					}
				};

				CreateRequest createRequest = new CreateRequest
				{
					ObjectType = new ObjectTypeRef { Guid = Guids.Model.OBJECT_TYPE },
					FieldValues = fieldValues
				};

				CreateResult result = await objectManager.CreateAsync(workspaceId, createRequest);
				returnValue = result.Object.ArtifactID;
			}

			return returnValue;
		}

		public async Task UpdateRelativity(IServicesMgr serviceManager, int workspaceId, int modelArtifactId)
		{
			using (IObjectManager objectManager = serviceManager.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{
				RelativityObjectRef relativityObject = new RelativityObjectRef { ArtifactID = modelArtifactId };
				List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>
				{
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Model.MODEL_GUID_FIELD },
						Value = ModelGuid.ToString()
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Model.SAS_URL_FIELD },
						Value = SasUrl
					}
				};

				UpdateRequest updateRequest = new UpdateRequest
				{
					Object = relativityObject,
					FieldValues = fieldValues
				};

				UpdateResult result = await objectManager.UpdateAsync(workspaceId, updateRequest);
			}
		}

		public async Task<Guid> ReadModelGuid(IServicesMgr serviceManager, int workspaceId, int modelId)
		{
			Guid returnValue = Guid.Empty;
			using (IObjectManager objectManager = serviceManager.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{

				IEnumerable<FieldRef> fieldRefs = new List<FieldRef>
				{
					new FieldRef
					{
						Guid = Guids.Model.MODEL_GUID_FIELD
					}
				};

				ReadRequest readRequest = new ReadRequest
				{
					Object = new RelativityObjectRef { ArtifactID = modelId },
					Fields = fieldRefs
				};

				ReadResult readReturnValue = await objectManager.ReadAsync(workspaceId, readRequest);
				ModelGuid = Guid.Parse(readReturnValue.Object.FieldValues.Find(x => x.Field.Guids.Contains(Guids.Model.MODEL_GUID_FIELD)).Value.ToString());
				returnValue = ModelGuid;
			}

			return returnValue;
		}

		public async Task<IEnumerable<int>> ReadDocumentsInSavedSeach(IServicesMgr serviceManager, int workspaceId, int savedSearchArtifactId)
		{
			List<int> returnVal = new List<int>();

			const int indexOfFirstDocumentInResult = 1; //1-based index of first document in query results to retrieve
			const int lengthOfResults = 100; //max number of results to return in this query call.

			string searchCondition =
				"('Artifact ID' IN SAVEDSEARCH @savedSearchId)".Replace("@savedSearchId", savedSearchArtifactId.ToString());


			QueryRequest queryRequest = new QueryRequest()
			{
				ObjectType = new ObjectTypeRef { Guid = Guids.Document.OBJECT_TYPE },
				Condition = searchCondition, //query condition syntax is used to build query condtion.  See Relativity's developer documentation for more details
				Fields = new List<global::Relativity.Services.Objects.DataContracts.FieldRef>()   //array of fields to return.  ArtifactId will always be returned.
				{
					new FieldRef { Name = "Artifact ID" },
				},
				IncludeIDWindow = false,
				RelationalField = null, //name of relational field to expand query results to related objects
				SampleParameters = null,
				SearchProviderCondition = null, //see documentation on building search providers
				QueryHint = "waitfor:5"
			};

			using (IObjectManager objectManager = serviceManager.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{
				QueryResultSlim queryResult = await objectManager.QuerySlimAsync(workspaceId, queryRequest, indexOfFirstDocumentInResult, lengthOfResults);
				if (queryResult.ResultCount > 0)
				{
					foreach (RelativityObjectSlim resultObject in queryResult.Objects)
					{
						returnVal.Add(resultObject.ArtifactID);
					}
				}
			}

			DocsInSearch = returnVal;

			return returnVal;
		}


	}
}
