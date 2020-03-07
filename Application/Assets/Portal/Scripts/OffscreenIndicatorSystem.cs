using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenIndicatorSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OffscreenIndicator indicatorPrefab = null;
    [SerializeField] private RectTransform holder = null;
    [SerializeField] private Camera cameraObject = null;
    [SerializeField] private Transform player = null;

    private Dictionary<Transform, OffscreenIndicator> indicators = new Dictionary<Transform, OffscreenIndicator>();

    #region Delegates
    public static Action<Transform> CreateIndicator = delegate { };
    public static Func<Transform, bool> CheckIfObjectIsVisible = null;
    #endregion

    private void OnEnable()
    {
        CreateIndicator += Create;
        CheckIfObjectIsVisible += ObjectIsVisible; 
    }

    private void OnDisable()
    {
        CreateIndicator -= Create;
        CheckIfObjectIsVisible -= ObjectIsVisible;
    }

    void Create(Transform target)
    {
        if(indicators.ContainsKey(target))
        {
            return;
        }

        OffscreenIndicator newIndicator = Instantiate(indicatorPrefab, holder);
        newIndicator.Register(target, player, new Action(() => { indicators.Remove(target); } ));

        indicators.Add(target, newIndicator);
    }

    bool ObjectIsVisible(Transform tObject)
    {
        Vector3 screenPoint = cameraObject.WorldToViewportPoint(tObject.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
