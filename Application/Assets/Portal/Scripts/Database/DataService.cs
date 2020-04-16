using SQLite4Unity3d;
using UnityEngine;
using System.Xml;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService  {

	private SQLiteConnection _connection;

	public DataService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);     

	}

	public void CreateDB(){
		_connection.CreateTable<PointOfInterest>();
		_connection.CreateTable<Route>();
		_connection.CreateTable<POI_Route>();

		//Make sure it's ran only once so that there are no duplicates
		LoadPointsOfInterest("point-of-interests");
	}

    //Load the point of interest from the google maps XML
    private void LoadPointsOfInterest(string filePath)
    {
		TextAsset textAsset = (TextAsset)Resources.Load(filePath);
		XmlDocument xmldoc = new XmlDocument();
		xmldoc.LoadXml(textAsset.text);

		XmlNodeList elements = xmldoc.GetElementsByTagName("Placemark");
		List<PointOfInterest> pois = new List<PointOfInterest>();
		for (int i = 0; i < elements.Count; i++)
		{
			XmlNode element = elements[i];
			string[] latLong = element["Point"]["coordinates"].InnerXml.Trim().Split(',');
			PointOfInterest poi = new PointOfInterest
			{
				Name = element["name"].InnerText,
				//Description = element["description"].InnerText,
				// For some reason the parsers takes Long,Lat coordinates, so the GMAPS output needs to be flipped
				Latitude = float.Parse(latLong[1]),
				Longitude = float.Parse(latLong[0]),
				Visited = false,
				Has360 = false,
				HasPortal = false,
				HasPedestal = true
			};
			pois.Add(poi);
		}
		_connection.InsertAll(pois.ToArray());
	}

	public Route getRoute(string routeName)
	{
		return _connection.Table<Route>().Where(route => route.Name.Contains(routeName)).FirstOrDefault();
	}

	public PointOfInterest getPointOfInterest(string pointOfInterestName)
	{
		return _connection.Table<PointOfInterest>().Where(poi => poi.Name.Contains(pointOfInterestName)).FirstOrDefault();
	}

	public IEnumerable<PointOfInterest> getPointsOfInterest()
	{
		return _connection.Table<PointOfInterest>();
	}

	//Should be using a JOIN, but couldn't figure out how to use it with the SQLite library
	public IEnumerable<PointOfInterest> getPointsOfInterestForRoute(Route route)
	{
		int routeId = route.Id;
		IEnumerable<POI_Route> poi_routes = _connection.Table<POI_Route>()
			.Where(poi_route => poi_route.RouteId.Equals(routeId));

		List<PointOfInterest> POIs = new List<PointOfInterest>();
		foreach(POI_Route poi_route in poi_routes)
		{
			PointOfInterest pointOfInterest = _connection.Table<PointOfInterest>().Where(poi => poi.Id.Equals(poi_route.PointOfInterestId)).First();
			POIs.Add(pointOfInterest);
		}

		return POIs;
	}

	public List<Route> getRoutesForPointOfInterest(PointOfInterest poi)
	{
		IEnumerable<POI_Route> poi_routes = _connection.Table<POI_Route>()
			.Where(poi_route => poi_route.PointOfInterestId.Equals(poi.Id));

		List<Route> routes = new List<Route>();
		foreach (POI_Route poi_route in poi_routes)
		{
			Route route = _connection.Table<Route>()
				.Where(x => x.Id.Equals(poi_route.RouteId)).First();
			routes.Add(route);
		}

		return routes;
	}
}
