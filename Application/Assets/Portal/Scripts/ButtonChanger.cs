using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChanger : MonoBehaviour
{
    public void ChangePath(string pathName)
    {
        print("hello");
        RouteController routeController = GameObject.FindObjectOfType<RouteController>();
        routeController._filePath = pathName;
    }
}
