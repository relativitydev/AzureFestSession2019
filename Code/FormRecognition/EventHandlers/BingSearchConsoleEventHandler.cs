using kCura.EventHandler;
using System;
using System.Collections.Generic;

namespace FormRecognition.EventHandlers
{
	[kCura.EventHandler.CustomAttributes.Description("Console EventHandler")]
	[System.Runtime.InteropServices.Guid("3F296ADB-E519-4664-AA52-7AA88634E3A8")]
	public class BingSearchConsoleEventHandler : kCura.EventHandler.ConsoleEventHandler
	{

		public override kCura.EventHandler.Console GetConsole(PageEvent pageEvent)
		{


			kCura.EventHandler.Console returnConsole = new kCura.EventHandler.Console()
			{ Items = new List<IConsoleItem>(), Title = "Bing Search" };
			string address = null;

			if (!ActiveArtifact.Fields[0].Value.IsNull)
			{
				address = ActiveArtifact.Fields[0].Value.Value.ToString();  //TODO: You will need to update this with the field ID for your own workspace 

                string noBreakingSpacesRemoved = address.Replace('\u00A0', ' ').Replace(" ", "%2b");
                returnConsole.Items.Add(new ConsoleButton() { Name = "Map", DisplayText = "Plot Location and Search", Enabled = true, OnClickEvent = "window.location.href = 'https://kcura-current-sandbox.relativity.one/Relativity/External.aspx?AppID=1463683&ArtifactID=1463683&DirectTo=%25applicationPath%25%2fCustomPages%2f0a9cd0ab-2924-4034-868d-9c97eb5cd85b%2fBingSearch.aspx?Address=" + noBreakingSpacesRemoved + "&SelectedTab=1067106'" });

            }

            Int32 currentWorkspaceArtifactID = Helper.GetActiveCaseID();


			return returnConsole;
		}

		public override void OnButtonClick(ConsoleButton consoleButton)
		{

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
				return retVal;
			}
		}
	}
}