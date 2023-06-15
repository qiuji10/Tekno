using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorToggler : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
