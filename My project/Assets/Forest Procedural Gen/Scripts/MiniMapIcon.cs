using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIcon : MonoBehaviour
{
    [HideInInspector] public Transform target;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }
}

