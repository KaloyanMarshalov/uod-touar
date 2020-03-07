using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenIndicator : MonoBehaviour
{
    private const float MAX_TIMER = 8.0f;

    private CanvasGroup canvasGroup = null;
    protected CanvasGroup CanvasGroup
    {
        get
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            return canvasGroup;
        }
    }

    private RectTransform rect = null;
    protected RectTransform Rect
    {
        get
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
                if (rect == null)
                {
                    rect = gameObject.AddComponent<RectTransform>();
                }
            }

            return rect;
        }
    }

    public Transform Target { get; protected set; } = null;
    private Transform player = null;
    private Action unRegister = null;

    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 targetPosition = Vector3.zero;

    public void Register(Transform target, Transform player, Action unRegister)
    {
        this.Target = target;
        this.player = player;
        this.unRegister = unRegister;


        StartCoroutine(RotateToTheTarget());
    }

    IEnumerator RotateToTheTarget()
    {
        while (enabled)
        {
            if(Target)
            {
                targetPosition = Target.position;
                targetRotation = Target.rotation;
            }

            Vector3 direction = player.position - targetPosition;

            targetRotation = Quaternion.LookRotation(direction);
            targetRotation.z = targetRotation.y;
            targetRotation.x = 0;
            targetRotation.y = 0;

            Vector3 northDirection = new Vector3(0, 0, player.eulerAngles.y);
            Rect.localRotation = targetRotation * Quaternion.Euler(northDirection);

            yield return null;
        }
    }
}
