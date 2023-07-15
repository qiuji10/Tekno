using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystem_Instance : MonoBehaviour
{
    public void SelectGameObject(GameObject selectedGameObject)
    {
        EventSystem.current.SetSelectedGameObject(selectedGameObject);
    }
}
