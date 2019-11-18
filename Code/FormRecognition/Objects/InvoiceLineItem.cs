using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace FormRecognition
{
	public class InvoiceLineItem
	{

		public readonly Guid ObjectType = Guid.Parse("E052E4DB-9ED0-471F-9D45-67260CE84DEB");

		public readonly Guid NameField = Guid.Parse("7696751A-3CFF-48EA-A80D-4DBB6B4B5EF9");
		public readonly Guid InvoiceField = Guid.Parse("C56BBAAF-0EC3-41D5-B4EB-DE4ED44AE621");
		public readonly Guid DescriptionField = Guid.Parse("B68484E5-8E46-47D9-86AE-BCDAB795C257");
		public readonly Guid RateField = Guid.Parse("B6CAD91B-465A-4154-86E0-B7C3FEF0BC4C");
		public readonly Guid QuantityField = Guid.Parse("8089F461-1E75-4B15-9CC6-A79AEC180CE2");
		public readonly Guid LineTotalField = Guid.Parse("96CF3AB9-3AD7-40FB-91EB-7CA8D7D44510");
		public readonly Guid AzureFormRecognizerForm = Guid.Parse("C56BBAAF-0EC3-41D5-B4EB-DE4ED44AE621");

		public string Name { get; set; }
		public string Description { get; set; }
		public string Rate { get; set; }
		public int Quantity { get; set; }
		public string LineTotal { get; set; }

		public async Task<int> CreateRelativity(IServicesMgr serviceManager, int workspaceId, int invoiceArtifactId)
		{
			int returnValue = 0;
			using (IObjectManager objectManager = serviceManager.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{

				List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>
				{
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = NameField },
						Value = Name
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = InvoiceField },
						Value = new RelativityObjectRef { ArtifactID = invoiceArtifactId }
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = DescriptionField },
						Value = Description
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = RateField },
						Value = Rate
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = QuantityField },
						Value = Quantity
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = LineTotalField },
						Value = LineTotal
					}
				};

				CreateRequest createRequest = new CreateRequest
				{
					ObjectType = new ObjectTypeRef { Guid = ObjectType },
					ParentObject = new RelativityObjectRef { ArtifactID = invoiceArtifactId },
					FieldValues = fieldValues
				};

				CreateResult result = await objectManager.CreateAsync(workspaceId, createRequest);
				returnValue = result.Object.ArtifactID;
			}

			return returnValue;
		}
	}
}
