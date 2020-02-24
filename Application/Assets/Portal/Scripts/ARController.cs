using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class ARController : MonoBehaviour
{
    /*Planes detected from the current frame*/
    private List<DetectedPlane> newTrackedPlanes = new List<DetectedPlane>();
    public GameObject gridPrefab;

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
            GameObject grid = Instantiate(gridPrefab, Vector3.zero, Quaternion.identity, transform);

            grid.GetComponent<DetectedPlaneVisualizer>().Initialize(newTrackedPlanes[i]);
        }
    }
}
