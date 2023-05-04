using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyManager : MonoBehaviour
{
   
    public void GetDontDestoryObjects()
    {
        NonDestructible[]destructibles = FindObjectsOfType<NonDestructible>();

        foreach (NonDestructible destructible in destructibles)
        {
            Destroy(destructible.gameObject);
        }
    }
}
