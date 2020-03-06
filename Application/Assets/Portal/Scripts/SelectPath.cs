using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPath : MonoBehaviour
{
    public Camera ARCamera;
    public GameObject PictureFrame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
            hitObject.transform.localScale += new Vector3(1, 1, 1);
        }
    }

    public void SpawnFrames()
    {
        int numberOfObjects = 5;
        float radius = 8f;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 pos = ARCamera.transform.position + new Vector3(x, 0, z);
            float angleDegrees = -angle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
            Instantiate(PictureFrame, pos, rot);
        }
    }
}
