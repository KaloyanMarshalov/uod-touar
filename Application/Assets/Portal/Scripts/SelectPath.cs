using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPath : MonoBehaviour
{
    private DataService dataService = new DataService("uod-toar.db");
    public Camera ARCamera;
    public GameObject PictureFrame;

    void Start()
    {
        SpawnFrames();
    }

    void Update()
    {
        Touch touch;
        // No interaction
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        Ray ray = ARCamera.ScreenPointToRay(touch.position);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            string pathName = hitObject.transform.Find("Text").GetComponent<TextMesh>().text;
            GameObject.Find("FrameContainer").Destroy();

            //HARDCODING PATH
            GameObject.Find("Manager").GetComponent<Manager>().currentRoute = dataService.getRoute(pathName);
            SceneManager.LoadScene("Location");
        }
    }

    public void SpawnFrames()
    {
        PointOfInterest poi = GameObject.Find("Manager").GetComponent<Manager>().currentPointOfInterest;
        Route[] routes = dataService.getRoutesForPointOfInterest(poi).ToArray();

        int numberOfObjects = routes.Length;
        float radius = 8f;
        GameObject frameContainer = new GameObject("FrameContainer");

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 pos = ARCamera.transform.position + new Vector3(x, 0, z);
            float angleDegrees = -angle * Mathf.Rad2Deg - 90;
            Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
            GameObject frame = Instantiate(PictureFrame, pos, rot) as GameObject;
            frame.transform.parent = frameContainer.transform;

            //Change the text above the frame
            frame.transform.Find("Text").GetComponent<TextMesh>().text = routes[i].Name;
            //TODO: Change shader image
        }
    }
}
