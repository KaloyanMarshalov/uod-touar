using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePinText : MonoBehaviour
{
    [SerializeField]
    GameObject text;

    void OnMouseDown()
    {
        TextMesh textMesh = text.GetComponent<TextMesh>();
        if (textMesh.text == "")
        {
            text.transform.localRotation = Camera.main.transform.localRotation;
            textMesh.text = gameObject.transform.parent.name;
        }
        else
        {
            textMesh.text = "";
        }
    }
}
