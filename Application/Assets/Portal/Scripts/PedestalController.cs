using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine.SceneManagement;

public class PedestalController : MonoBehaviour
{
    /*Planes detected from the current frame*/
    private List<DetectedPlane> newTrackedPlanes = new List<DetectedPlane>();
    public GameObject GridPrefab;
    public GameObject ARCamera;
    public GameObject PedestalPrefab;

    private GameObject Pedestal;

    // Start is called before the first frame update
    void Start()
    {
        //PointOfInterest poi = GameObject.Find("Manager").GetComponent<Manager>().currentPointOfInterest;
        /* Instantiate(Resources.Load<GameObject>("Prefabs/Pedestal Prefabs/Duncan of Jordanstone College of Art & Design"));
         GameObject.Find("Duncan of Jordanstone College of Art & Design(Clone)").SetActive(false);*/
        Pedestal = Instantiate(PedestalPrefab);
        Pedestal.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        /*Close app if session isn't tracking */
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        Session.GetTrackables<DetectedPlane>(newTrackedPlanes, TrackableQueryFilter.New);

        for (int i = 0; i < newTrackedPlanes.Count; i++)
        {
            GameObject grid = Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform);

            grid.GetComponent<DetectedPlaneVisualizer>().Initialize(newTrackedPlanes[i]);
        }

        Touch touch;
        // No interaction
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
        {
            Pedestal.SetActive(true);
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
            Pedestal.transform.position = hit.Pose.position;
            Pedestal.transform.rotation = hit.Pose.rotation;

            Vector3 cameraPosition = ARCamera.transform.position;
            cameraPosition.y = hit.Pose.position.y;
            Pedestal.transform.LookAt(cameraPosition, Pedestal.transform.up);
            Pedestal.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            Pedestal.transform.parent = anchor.transform;
        }
    }
}
