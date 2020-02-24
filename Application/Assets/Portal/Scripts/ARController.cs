using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class ARController : MonoBehaviour
{
    /*Planes detected from the current frame*/
    private List<DetectedPlane> newTrackedPlanes = new List<DetectedPlane>();
    public GameObject GridPrefab;
    public GameObject Door;
    public GameObject ARCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*Close app if session isn't tracking */
        if(Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        Session.GetTrackables<DetectedPlane>(newTrackedPlanes, TrackableQueryFilter.New); 

        for(int i = 0; i < newTrackedPlanes.Count; i++)
        {
            GameObject grid = Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform);

            grid.GetComponent<DetectedPlaneVisualizer>().Initialize(newTrackedPlanes[i]);
        }

        Touch touch;
        // No interaction
        if(Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        if(Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
        {
            Door.SetActive(true);
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
            Door.transform.position = hit.Pose.position;
            Door.transform.rotation = hit.Pose.rotation;

            Vector3 cameraPosition = ARCamera.transform.position;
            cameraPosition.y = hit.Pose.position.y;
            Door.transform.LookAt(cameraPosition, Door.transform.up);

            Door.transform.parent = anchor.transform;
        }
    }
}
