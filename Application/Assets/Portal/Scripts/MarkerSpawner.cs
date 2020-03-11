using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using System.Xml;

public class MarkerSpawner : MonoBehaviour
{
	public string _fileName;

	[SerializeField]
	AbstractMap _map;

	[SerializeField]
	[Geocode]
	string[] _locationStrings;
	Vector2d[] _locations;

	[SerializeField]
	float _spawnScale = 200f;

	[SerializeField]
	GameObject _markerPrefab;

	[SerializeField]
	Transform _markerHolder;

	List<GameObject> _spawnedObjects;

	void Start()
	{
		_locationStrings = LoadCoordinateFromXMLFile(_fileName);

		_locations = new Vector2d[_locationStrings.Length];
		_spawnedObjects = new List<GameObject>();
		for (int i = 0; i < _locationStrings.Length; i++)
		{
			var locationString = _locationStrings[i];
			_locations[i] = Conversions.StringToLatLon(locationString);
			var instance = Instantiate(_markerPrefab, _markerHolder);
			instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
			instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			_spawnedObjects.Add(instance);
		}
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
