using UnityEngine;
using Mapbox.Directions;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using System.Collections;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity;
using Mapbox.Json;
using Mapbox.Utils.JsonConverters;

public class RouteController : MonoBehaviour
{
	private DataService dataService = new DataService("uod-toar.db");

	[SerializeField]
	AbstractMap _map;

	[SerializeField]
	MeshModifier[] MeshModifiers;
	[SerializeField]
	Material _material;

	[SerializeField]
	Transform[] _waypoints;
	[SerializeField]
	Transform _waypointsContainer;
	private List<Vector3> _cachedWaypoints;

	[SerializeField]
	[Range(1, 10)]
	private float UpdateFrequency = 2;

	private Directions _directions;
	private int _counter;

	GameObject _directionsGO;
	private bool _recalculateNext;

	protected virtual void Awake()
	{
		//_directions = MapboxAccess.Instance.Directions;
		//_map.OnInitialized += Query;
		_map.OnUpdated += Query;
	}

	public void Start()
	{
		//Fill out waypoints from the waypoints generator
		List<Transform> markers = new List<Transform>();
		for (int i = 0; i < _waypointsContainer.childCount; i++)
		{
			markers.Add(_waypointsContainer.GetChild(i));
		}
		_waypoints = markers.ToArray();

		_cachedWaypoints = new List<Vector3>(_waypoints.Length);
		foreach (var item in _waypoints)
		{
			_cachedWaypoints.Add(item.position);
		}
		_recalculateNext = false;

		foreach (var modifier in MeshModifiers)
		{
			modifier.Initialize();
		}

		StartCoroutine(QueryTimer());
	}

	protected virtual void OnDestroy()
	{
		_map.OnInitialized -= Query;
		/*_map.OnUpdated -= Query;*/
	}

	void Query()
	{
		var count = _waypoints.Length;
		var wp = new Vector2d[count];
		for (int i = 0; i < count; i++)
		{
			wp[i] = _waypoints[i].GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
		}
		
		string pathName = PlayerPrefs.GetString("path");

		if (PlayerPrefs.GetString("path").Length > 0)
		{
			DrawPath(pathName);
		}
	}

	public IEnumerator QueryTimer()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.01f);
			for (int i = 0; i < _waypoints.Length; i++)
			{
				if (_waypoints[i].position != _cachedWaypoints[i])
				{
					_recalculateNext = true;
					_cachedWaypoints[i] = _waypoints[i].position;
				}
			}

			if (_recalculateNext)
			{
				Query();
				_recalculateNext = false;
			}
		}
	}

	void DrawPath(string pathName)
	{
		string json = "";
		Route route = dataService.getRoute(pathName);
		json = route.MapboxRouteJSON;

		var response = JsonConvert.DeserializeObject<DirectionsResponse>(json, JsonConverters.Converters);

		if (response == null || null == response.Routes || response.Routes.Count < 1)
		{
			return;
		}

		var meshData = new MeshData();
		var dat = new List<Vector3>();
		foreach (var point in response.Routes[0].Geometry)
		{
			dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
		}

		var feat = new VectorFeatureUnity();
		feat.Points.Add(dat);

		foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
		{
			mod.Run(feat, meshData, _map.WorldRelativeScale);
		}

		CreateGameObject(meshData);
	}

	GameObject CreateGameObject(MeshData data)
	{
		if (_directionsGO != null)
		{
			_directionsGO.Destroy();
		}
		_directionsGO = new GameObject("direction waypoint " + " entity");
		var mesh = _directionsGO.AddComponent<MeshFilter>().mesh;
		mesh.subMeshCount = data.Triangles.Count;

		mesh.SetVertices(data.Vertices);
		_counter = data.Triangles.Count;
		for (int i = 0; i < _counter; i++)
		{
			var triangle = data.Triangles[i];
			mesh.SetTriangles(triangle, i);
		}

		_counter = data.UV.Count;
		for (int i = 0; i < _counter; i++)
		{
			var uv = data.UV[i];
			mesh.SetUVs(i, uv);
		}

		mesh.RecalculateNormals();
		_directionsGO.AddComponent<MeshRenderer>().material = _material;
		return _directionsGO;
	}
}
