<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BingSearch.aspx.cs" Inherits="Kramerica_BingSearch.BingSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>

    <form id="form1" runat="server">
        <h1 id="PageTopper" runat="server" style="text-align:center; background-color:lightgray">RelativityFest is Fun!</h1>
        <input type="hidden" id="ctlPartialAddress" runat="server" />

        <script type='text/javascript' src='https://www.bing.com/api/maps/mapcontrol?callback=loadMapScenario'></script>
        <script type='text/javascript'>
             function loadMapScenario() {
                 var map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
                     credentials: '' //Add your own credentials here
                 });
                 Microsoft.Maps.loadModule('Microsoft.Maps.Search', function () {
                     var searchManager = new Microsoft.Maps.Search.SearchManager(map);
                     var requestOptions = {
                         bounds: map.getBounds(),
                         where: document.getElementById('<%= ctlPartialAddress.ClientID %>').value,
                         callback: function (answer, userData) {
                             map.setView({
                                 bounds: answer.results[0].bestView
                             });
                             map.entities.push(new Microsoft.Maps.Pushpin(answer.results[0].location));
                             map.zoom
                         }
                     };
                     searchManager.geocode(requestOptions);
                 });
                 
             }
        </script>
        <div></div>
        <div id="MapAndSearchContainer" >
        <div id='myMap'/>
        <div id="bingSearch" runat="server" ></div>
        </div>
    </form>
        
        </body>
</html>
