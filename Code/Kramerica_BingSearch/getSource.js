function getSrc() {
    var mySrc = 'https://www.bing.com/api/maps/mapcontrol?key=' + '<%= subsriptionKey.ClientID %>' +'&callback=loadMapScenario'
    return mySrc;
}