using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class PointLightSelection : MonoBehaviour
{
    [Button]
    public void SelectPointLight()
    {
        // Get all Point Lights in the scene
        Light[] pointLights = FindObjectsOfType<Light>();

        // List to store selected lights
        List<Object> selectedObjects = new List<Object>();

        // Select all Point Lights
        foreach (Light pointLight in pointLights)
        {
            if (pointLight.type == LightType.Point)
                selectedObjects.Add(pointLight.gameObject);
        }

        // Assign the list of selected lights to the selection
        UnityEditor.Selection.objects = selectedObjects.ToArray();
    }

    [Button]
    public void SelectSpotLight()
    {
        // Get all Point Lights in the scene
        Light[] pointLights = FindObjectsOfType<Light>();

        // List to store selected lights
        List<Object> selectedObjects = new List<Object>();

        // Select all Spot Lights
        foreach (Light pointLight in pointLights)
        {
            if (pointLight.type == LightType.Spot)
                selectedObjects.Add(pointLight.gameObject);
        }

        // Assign the list of selected lights to the selection
        UnityEditor.Selection.objects = selectedObjects.ToArray();
    }
}
