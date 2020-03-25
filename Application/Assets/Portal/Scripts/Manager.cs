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
    [SerializeField]
    Sprite _cameraImage;
    [SerializeField]
    Sprite _mapImage;
    private GameObject[] arButtons;
    private Vector2d cachedLatLong;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Location");
        arButtons = GameObject.FindGameObjectsWithTag("ExtraARButtons");
        changeARButtonsState(true);
        GameObject.Find("SelectPathButton").SetActive(false);
        GameObject.Find("ARScenesButton").SetActive(false);
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Location")
        {
            var locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
     
            if (!cachedLatLong.Equals(locationProvider.CurrentLocation.LatitudeLongitude))
            {
                cachedLatLong = locationProvider.CurrentLocation.LatitudeLongitude;
                checkIfNearLocation(cachedLatLong);
            }
        }
    }

    private bool checkIfNearLocation(Vector2d locationLatLong)
    {
        Vector2d[] locationsOnMap = GameObject.Find("MarkerHolder").GetComponent<MarkerSpawner>()._locations;
        int distance = (int) DistanceCalculator.calculateDistance(locationLatLong, locationsOnMap[9]);
        print(distance + " meters");
        return false;
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
    public void changeARButtonsState(bool turnButtonsOff)
    {
        bool state = turnButtonsOff ? false : !arButtons[0].activeSelf;

        foreach(GameObject button in arButtons)
        {
            button.SetActive(state);
        }
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        changeARButtonsState(true);
    }
}
