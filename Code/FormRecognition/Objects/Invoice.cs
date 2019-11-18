using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.FormRecognizer.Models;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;


namespace FormRecognition
{
	public class Invoice
	{
		public string To { get; set; }
		public string From { get; set; }
		public DateTime DateOfIssue { get; set; }
		public DateTime DueDate { get; set; }
		public string TotalAmountDue { get; set; }
		public List<InvoiceLineItem> LineItems { get; set; }


		public Invoice ConvertAnalyzeResultToInvoice(AnalyzeResult result, IHelper helper)
		{
			Invoice invoice = new Invoice();
			List<string> values = new List<string>();
			string field;

			try
			{
				Dictionary<string, string> keyValues = new Dictionary<string, string>();

				foreach (ExtractedPage page in result.Pages)
				{
					foreach (ExtractedKeyValuePair pair in page.KeyValuePairs)
					{
						string key = pair.Key[0].Text.Replace(" ", "_").Replace("(", "_").Replace(")", "_").Trim();
						switch (key)
						{
							case "To:":
								values = pair.Value.Select(p => p.Text).ToList();
								field = string.Join(" ", values).Trim();
								invoice.To = field;
								values.Clear();
								break;
							case "From:":
								values = pair.Value.Select(p => p.Text).ToList();
								field = string.Join(" ", values).Trim();
								invoice.From = field;
								values.Clear();
								break;
							case "Due_Date:":
								values = pair.Value.Select(p => p.Text).ToList();
								field = string.Join(" ", values).Trim();
								invoice.DueDate = DateTime.Parse(field);
								values.Clear();
								break;
							case "Date_of_Issue:":
								values = pair.Value.Select(p => p.Text).ToList();
								field = string.Join(" ", values).Trim();
								invoice.DateOfIssue = DateTime.Parse(field);
								values.Clear();
								break;
							case "Total_Amount_Due":
								values = pair.Value.Select(p => p.Text).ToList();
								field = string.Join(" ", values).Trim();
								invoice.TotalAmountDue = field;
								values.Clear();
								break;
							default:
								break;
						}



					}

					//getting invoice line items here
					foreach (ExtractedTable table in page.Tables)
					{
						int columnCount = 0;
						DataTable tbl = new DataTable();
						foreach (ExtractedTableColumn column in table.Columns)
						{
							tbl.Columns.Add(table.Columns[columnCount].Header[0].Text, typeof(string));
							if (columnCount == 0)
							{
								for (int i = 0; i < column.Entries.Count; ++i)
								{
									tbl.Rows.Add(column.Entries[i][0].Text);
								}

							}
							else
							{
								for (int i = 0; i < column.Entries.Count; ++i)
								{
									tbl.Rows[i][table.Columns[columnCount].Header[0].Text] = column.Entries[i][0].Text;
									tbl.AcceptChanges();
								}
							}
							columnCount += 1;
						}

						List<InvoiceLineItem> listItems = new List<InvoiceLineItem>();

						const int descriptionColumnIndex = 0;
						const int rateColumnIndex = 1;
						const int quantityColumnIndex = 2;
						const int lineTotalColumnIndex = 3;
						listItems = tbl.Select().Select(row =>
						{
							InvoiceLineItem lineItem = new InvoiceLineItem();
							lineItem.Description = row[descriptionColumnIndex].ToString();
							lineItem.Rate = SanitizeCurrency(row[rateColumnIndex].ToString());
							lineItem.Quantity = Convert.ToInt32(row[quantityColumnIndex].ToString()) as int? ?? 0;
							lineItem.LineTotal = SanitizeCurrency(row[lineTotalColumnIndex].ToString());
							return lineItem;
						}).ToList();
						invoice.LineItems = listItems;
					}
				}
			}
			catch (Exception ex)
			{
				IAPILog logger = helper?.GetLoggerFactory()?.GetLogger()?.ForContext<Invoice>();
				logger?.LogError(ex, "Error converting AnalyzeResult to Invoice");
			}

			return invoice;

		}

		private string SanitizeCurrency(string input)
		{
			return input.Replace("$", "").Trim();

		}

		public async Task UpdateRelativity(IServicesMgr serviceManager, int workspaceId, int invoiceArtifactId)
		{
			using (IObjectManager objectManager = serviceManager.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{
				RelativityObjectRef relativityObject = new RelativityObjectRef { ArtifactID = invoiceArtifactId };
				List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>
				{
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Invoice.TO_FIELD },
						Value = To
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Invoice.FROM_FIELD },
						Value = From
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Invoice.DATE_OF_ISSUE_FIELD },
						Value = DateOfIssue
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Invoice.DUE_DATE_FIELD },
						Value = DueDate
					},
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Invoice.TOTAL_AMOUNT_DUE_FIELD },
						Value = TotalAmountDue
					}
				};

				UpdateRequest updateRequest = new UpdateRequest
				{
					Object = relativityObject,
					FieldValues = fieldValues
				};

				UpdateResult result = await objectManager.UpdateAsync(workspaceId, updateRequest);

				if (invoiceArtifactId > 0)
				{
					foreach (InvoiceLineItem lineItem in LineItems)
					{
						await lineItem.CreateRelativity(serviceManager, workspaceId, invoiceArtifactId);
					}
				}
			}
		}

		public async Task<int> CreateInvoice(IServicesMgr serviceManager, int workspaceId, string name)
		{
			int returnValue = 0;
			using (IObjectManager objectManager = serviceManager.CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{

				List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>
				{
					new FieldRefValuePair
					{
						Field = new FieldRef() { Guid = Guids.Invoice.NAME_FIELD },
						Value = name
					}
				};

				CreateRequest createRequest = new CreateRequest
				{
					ObjectType = new ObjectTypeRef { Guid = Guids.Invoice.OBJECT_TYPE },
					FieldValues = fieldValues
				};

				CreateResult result = await objectManager.CreateAsync(workspaceId, createRequest);
				returnValue = result.Object.ArtifactID;
			}

			if (returnValue > 0)
			{
				foreach (InvoiceLineItem lineItem in LineItems)
				{
					await lineItem.CreateRelativity(serviceManager, workspaceId, returnValue);
				}
			}
			return returnValue;
		}
	}
}
