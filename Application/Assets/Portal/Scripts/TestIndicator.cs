using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Register", 0);
    }
    void Register()
    {
        if (!OffscreenIndicatorSystem.CheckIfObjectIsVisible(this.transform))
        {
            OffscreenIndicatorSystem.CreateIndicator(this.transform);
        }
    }
}
