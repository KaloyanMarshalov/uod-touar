using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePinText : MonoBehaviour
{
    [SerializeField]
    GameObject text;

    void OnMouseDown()
    {
        text.transform.localRotation = Camera.main.transform.localRotation;
        text.GetComponent<TextMesh>().text = gameObject.transform.parent.name;
    }
}
