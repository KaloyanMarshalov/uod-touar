using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DoorManager : MonoBehaviour
{
    public GameObject mainCamera;

    public GameObject Sponza;

    private Material[] SponzaMaterials;

    private Material PortalPlaneMaterial;

    // Start is called before the first frame update
    void Start()
    {
        SponzaMaterials = Sponza.GetComponent<Renderer>().sharedMaterials;
        PortalPlaneMaterial = GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void OnTriggerStay(Collider collider)
    {
        Vector3 cameraPositionInDoorSpace = transform.InverseTransformPoint(mainCamera.transform.position);
        
        // Player is inside portal
        if (cameraPositionInDoorSpace.y <= 0.0f)
        {
            for (int i = 0; i < SponzaMaterials.Length; i++)
            {
                SponzaMaterials[i].SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }

            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Front);
        }
        //Outside the portal and within half a metre
        else if (cameraPositionInDoorSpace.y < 0.5f)
        {
            for (int i = 0; i < SponzaMaterials.Length; i++)
            {
                SponzaMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }

            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Off);
        }
        //Outside the portal by more than half a metre
        else
        {
            for (int i = 0; i < SponzaMaterials.Length; i++)
            {
                SponzaMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }

            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Back);
        }
    }
}
