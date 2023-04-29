using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnTransformParentChanged()
    {
        if (transform.parent == null) DontDestroyOnLoad(gameObject);
    }
}
