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
				Visited = false
			};
			pois.Add(poi);
		}
		_connection.InsertAll(pois.ToArray());
	}
}
