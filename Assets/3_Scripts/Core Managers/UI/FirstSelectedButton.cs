using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstSelectedButton : MonoBehaviour
{
    private void Start()
    {
        EventSystem.current.firstSelectedGameObject = this.gameObject;
    }
}
