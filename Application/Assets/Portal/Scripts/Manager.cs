using Mapbox.Json.Linq;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private const int DISTANCE_FROM_TARGET = 30;    //Change to 20 when finished

    [SerializeField]
    Sprite _cameraImage;
    [SerializeField]
    Sprite _mapImage;
    [SerializeField]
    GameObject _UITextbox;

    private GameObject[] arButtons;
    private GameObject[] pathAndARSceneButtons;
    private Vector2d cachedLatLong;
    private DataService dataService;
    public PointOfInterest currentPointOfInterest;
    public Route currentRoute;

    private void Awake()
    {
        arButtons = GameObject.FindGameObjectsWithTag("ExtraARButtons");
        pathAndARSceneButtons = GameObject.FindGameObjectsWithTag("PathAndARSceneButtons");
        turnOffButtons(arButtons);
        turnOffButtons(pathAndARSceneButtons);
        dataService = new DataService("uod-toar.db");
        currentRoute = dataService.getRoute("Formal and Applied Sciences");
    }

    private void Start()
    {
        SceneManager.LoadScene("Location");
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Location")
        {
            ILocationProvider locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            Vector2d currentLatLong = locationProvider.CurrentLocation.LatitudeLongitude;

            if ((float)cachedLatLong.x != (float)currentLatLong.x || 
                (float)cachedLatLong.y != (float)currentLatLong.y)
            {
                cachedLatLong = currentLatLong;
                checkIfNearLocation(cachedLatLong); 
            }
        }
    }

    private void checkIfNearLocation(Vector2d locationLatLong)
    {
        GameObject markerHolder = GameObject.Find("MarkerHolder");
        Vector2d[] locationsOnMap = markerHolder.GetComponent<MarkerSpawner>()._locations;

        for(int i = 0; i < locationsOnMap.Length; i++)
        {
            int distance = (int)DistanceCalculator.calculateDistance(locationLatLong, locationsOnMap[i]);

            if (distance < DISTANCE_FROM_TARGET)
            {
                currentPointOfInterest = dataService.getPointOfInterest(markerHolder.transform.GetChild(i).name);
                
                var connectedRoutes = dataService.getRoutesForPointOfInterest(currentPointOfInterest);
                turnOnButtons(pathAndARSceneButtons);

                //Hide the Routes button since we are on a path and not on a hub
                //if (currentRoute != null && connectedRoutes.Count <= 1)
                //{
                    GameObject.Find("SelectPathButton").SetActive(false);
                //}

                string message = "You have arrived at: " + currentPointOfInterest.Name;
                _UITextbox.GetComponent<Text>().text = message;
                Handheld.Vibrate();
                return;
            }
            else
            {
                //we've left a location, so mark it as visited.
                if(currentPointOfInterest != null && currentRoute != null)
                {
                    dataService.updatePointOfInterestFlag(currentPointOfInterest, "Visited", true);
                }

                _UITextbox.GetComponent<Text>().text = "Please make your way to one of the locations.";
                turnOffButtons(pathAndARSceneButtons);
                turnOffButtons(arButtons);
            }
        }
    }

    //Switch between Camera and map scene
    public void switchScenes()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        GameObject buttonImage = GameObject.Find("UI Button Image");
        if(sceneName == "SelectPath" || sceneName == "PlainCamera")
        {
            SceneManager.LoadScene("Location");
            GameObject.Find("ScenesButton").GetComponentsInChildren<Image>()[1].sprite = _cameraImage;
        }
        else
        {
            SceneManager.LoadScene("PlainCamera");
            GameObject.Find("ScenesButton").GetComponentsInChildren<Image>()[1].sprite = _mapImage;
        }
    }

    public void toggleARButtons()
    {
        if (arButtons[0].activeSelf == true)
        {
            turnOffButtons(arButtons);
        }
        else
        {
            turnOnButtons(arButtons);
            //Grey out the AR buttons for which we don't have a scene.
            foreach (GameObject button in arButtons)
            {
                if ((button.name.Contains("Portal") && currentPointOfInterest.HasPortal) ||
                    (button.name.Contains("360") && currentPointOfInterest.Has360) ||
                    (button.name.Contains("Pedestal") && currentPointOfInterest.HasPedestal))
                {
                    button.GetComponent<Button>().interactable = true;
                    button.transform.Find("UI Button Image").GetComponent<Image>().color = new Color(255, 255, 255);
                }
                else
                {
                    button.GetComponent<Button>().interactable = false;
                    button.transform.Find("UI Button Image").GetComponent<Image>().color = new Color(0, 0, 0);
                }
            }
        }
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        turnOffButtons(arButtons);
    }

    private void turnOffButtons(GameObject[] buttons)
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
    }

    private void turnOnButtons(GameObject[] buttons)
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
    }
}
