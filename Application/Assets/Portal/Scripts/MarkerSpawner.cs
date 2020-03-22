using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using System.Xml;

public class MarkerSpawner : MonoBehaviour
{
	private DataService dataService = new DataService("uod-toar.db");
	public string _fileName;

	[SerializeField]
	AbstractMap _map;

	[SerializeField]
	[Geocode]
	Vector2d[] _locations;

	[SerializeField]
	float _spawnScale = 200f;

	[SerializeField]
	GameObject _markerPrefab;

	[SerializeField]
	Transform _markerHolder;

	[SerializeField]
	GameObject _directionObject;

	List<GameObject> _spawnedObjects;

	//Spawn all locations from the DB
	void Start()
	{
		var pointsOfInterest = dataService.getPointsOfInterest();
		_spawnedObjects = new List<GameObject>();
		List<Vector2d> locations = new List<Vector2d>();

		foreach (PointOfInterest pointOfInterest in pointsOfInterest)
		{
			string locationString = pointOfInterest.Latitude + ", " + pointOfInterest.Longitude;
			Vector2d convertedLocationString = Conversions.StringToLatLon(locationString);

			var instance = Instantiate(_markerPrefab, _markerHolder);
			instance.transform.localPosition = _map.GeoToWorldPosition(convertedLocationString, true);
			instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			_spawnedObjects.Add(instance);
			locations.Add(convertedLocationString);
		}

		_locations = locations.ToArray();
	}

	private void Update()
	{
		int count = _spawnedObjects.Count;
		for (int i = 0; i < count; i++)
		{
			var spawnedObject = _spawnedObjects[i];
			var location = _locations[i];
			spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
			spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
		}
	}

	private string[] LoadCoordinateFromXMLFile(string filename)
	{
		List<string> coordinates = new List<string>();

		TextAsset textAsset = (TextAsset)Resources.Load(filename);
		XmlDocument xmldoc = new XmlDocument();
		xmldoc.LoadXml(textAsset.text);

		XmlNodeList elements = xmldoc.GetElementsByTagName("coordinates");
		for(int i = 0; i < elements.Count; i++)
		{
			string[] values = elements[i].InnerXml.Trim().Split(',');
			//For some reason the parsers takes Long, Lat coordinates, so the GMAPS output needs to be flipped
			string latLong = values[1] + ", " + values[0];
			coordinates.Add(latLong);
		}

		return coordinates.ToArray();
	}
}
