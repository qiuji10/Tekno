using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyChildrens(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (Application.isPlaying)
                GameObject.Destroy(child.gameObject);
            else
                GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
