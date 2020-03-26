using Mapbox.Examples;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private const int DISTANCE_FROM_TARGET = 30;

    [SerializeField]
    Sprite _cameraImage;
    [SerializeField]
    Sprite _mapImage;
    [SerializeField]
    GameObject _UITextbox;

    private GameObject[] arButtons;
    private GameObject[] pathAndARSceneButtons;
    private Vector2d cachedLatLong;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Location");
        arButtons = GameObject.FindGameObjectsWithTag("ExtraARButtons");
        pathAndARSceneButtons = GameObject.FindGameObjectsWithTag("PathAndARSceneButtons");
        changeButtonsState(true, arButtons);
        changeButtonsState(true, pathAndARSceneButtons);
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Location")
        {
            ILocationProvider locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;

            if (!cachedLatLong.Equals(locationProvider.CurrentLocation.LatitudeLongitude))
            {
                cachedLatLong = locationProvider.CurrentLocation.LatitudeLongitude;
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
                //TODO: activate buttons and path specific stuff.
                changeButtonsState(false, pathAndARSceneButtons);
                string message = "You have arrived at: " + markerHolder.transform.GetChild(i).name + "!";
                _UITextbox.GetComponent<Text>().text = message;
                Handheld.Vibrate();
                return;
            }
            else
            {
                _UITextbox.GetComponent<Text>().text = "Please make your way to one of the locations.";
                changeButtonsState(true, pathAndARSceneButtons);
            }
        }
    }

    //Switch between Camera and map scene
    public void switchScenes()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        GameObject buttonImage = GameObject.Find("UI Button Image");
        if(sceneName == "SelectPath")
        {
            SceneManager.LoadScene("Location");
            EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1].sprite = _cameraImage;
        }
        else
        {
            SceneManager.LoadScene("SelectPath");
            EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1].sprite = _mapImage;
        }
    }

    //Show or hide the extra AR UI buttons
    public void changeButtonsState(bool turnButtonsOff, GameObject[] gameObjects)
    {
        bool state = turnButtonsOff ? false : !gameObjects[0].activeSelf;

        foreach(GameObject button in gameObjects)
        {
            button.SetActive(state);
        }
    }

    public void changeARButtonsState()
    {
        changeButtonsState(false, arButtons);
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        changeButtonsState(true, arButtons);
    }
}
