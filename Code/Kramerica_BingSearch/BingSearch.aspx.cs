using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Kramerica_BingSearch
{
    public partial class BingSearch : System.Web.UI.Page
    {
        private string fullAddress { get; set; }
        private string partialAddress { get; set; }
        private WebSearchClient client { get; set; }
        private string websearchClientSubscriptionKey = ""; //TODO: Add your own credentials here
        private string bingMapsMasterKey = ""; //TODO: Add your own credentials here
        private string bingMapsQueryKey = ""; //TODO: Add your own credentials here
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!this.IsPostBack)
            {
                ParseAddress();
                ctlPartialAddress.Value = partialAddress;
                
                GetClient();
                var results = SearchBingForAddressInfo();
                PutWebSearchResultsInDiv(results);
            }
        }

        private void ParseAddress()
        {
            string encodedAddress = string.Empty;
            if (Request.QueryString["Address"] != null)
            {
                fullAddress = Request.QueryString["Address"];
            }

            var match = Regex.Match(fullAddress, @"\d+?[A-Za-z]*\s?\w*\s?\w+?\s?\w{2}\w*\s*?\w*");
            if (match.Success)
            {
                int index = match.Index;
                partialAddress = fullAddress.Substring(index, fullAddress.Length - index);

                PageTopper.InnerText = "Displaying results for:  " + partialAddress;

            }
        }
       
        private void GetClient()
        {
            client = new WebSearchClient(new ApiKeyServiceClientCredentials(websearchClientSubscriptionKey));
        }

        private SearchResponse SearchBingForAddressInfo()
        {
            var results = Task.Run(async () => await client.Web.SearchAsync(query: fullAddress, offset: 10, count: 20)).Result;
            return results;
        }

        private void PutWebSearchResultsInDiv(SearchResponse results)
        {
            HtmlGenericControl mainSearchDiv = new HtmlGenericControl("DIV");
            mainSearchDiv.ID = "webSearchDiv";
            mainSearchDiv.Style.Add(HtmlTextWriterStyle.Height, "500px");
            mainSearchDiv.Style.Add(HtmlTextWriterStyle.Width, "700px");
            mainSearchDiv.Style.Add("border", "1px solid black");
            mainSearchDiv.Style.Add("overflow", "scroll");
            mainSearchDiv.Style.Add("background-color", "azure");
            mainSearchDiv.InnerText = "Web Search Results";
            this.Controls.Add(mainSearchDiv);

            for (int i = 0; i < results.WebPages.Value.Count; i++)
            {
                HtmlGenericControl subDiv = new HtmlGenericControl("DIV");
                subDiv.ID = string.Format("subDiv{0}", i);
                subDiv.Style.Add("border", "1px solid black");
                //subDiv.InnerText = "SubDiv";
                mainSearchDiv.Controls.Add(subDiv);
                int divCounter = 1;

                for (int l = 0; l < 3; l++)
                {
                    switch (l)
                    {
                        case 0:
                            HtmlGenericControl lineItemDiv0 = new HtmlGenericControl("DIV");
                            lineItemDiv0.ID = string.Format("lineItemDiv{0}url", i);
                            lineItemDiv0.InnerText = results.WebPages.Value[i].Name;
                            lineItemDiv0.Style.Add("color", "red");
                            lineItemDiv0.Style.Add("font-weight", "bold");
                            subDiv.Controls.Add(lineItemDiv0);
                            break;
                        case 1:
                            HtmlAnchor lineItemDiv1 = new HtmlAnchor();
                            lineItemDiv1.ID = string.Format("lineItemDiv{0}url", i);
                            lineItemDiv1.InnerText = results.WebPages.Value[i].Url;
                            lineItemDiv1.HRef = results.WebPages.Value[i].Url;
                            lineItemDiv1.Style.Add("font-weight", "bold");
                            subDiv.Controls.Add(lineItemDiv1);
                            break;
                        case 2:
                            HtmlGenericControl lineItemDiv2 = new HtmlGenericControl("DIV");
                            lineItemDiv2.ID = string.Format("lineItemDiv{0}url", i);
                            lineItemDiv2.InnerText = results.WebPages.Value[i].Snippet;
                            subDiv.Controls.Add(lineItemDiv2);
                            break;
                        default:
                            break;
                    }
                    
                }
                divCounter += 1;
            }
        }
    }
}